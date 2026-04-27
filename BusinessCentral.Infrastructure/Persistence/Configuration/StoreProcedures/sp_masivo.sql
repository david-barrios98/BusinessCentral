CREATE OR ALTER PROCEDURE [config].[sp_check_tenant_access]
    @company_id INT,
    @module_name VARCHAR(50) = NULL -- Puede ser el Code del módulo
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CurrentDate DATETIME = GETUTCDATE();

    -- 1. Verificar si la compañía existe y está activa
    IF NOT EXISTS (SELECT 1 FROM [business].[Companies] WITH (NOLOCK) WHERE Id = @company_id AND Active = 1)
BEGIN
SELECT 'CompanyDisabled' AS AccessStatus;
RETURN;
END

    -- 2. Verificar si tiene una suscripción vigente y activa
    -- Nota: Usamos el campo Status de tu tabla CompanySubscription
    DECLARE @PlanId INT;
SELECT @PlanId = MembershipPlanId
FROM [config].[CompanySubscription] WITH (NOLOCK)
WHERE CompanyId = @company_id
  AND @CurrentDate BETWEEN StartDate AND EndDate
  AND [Status] = 'Active'; -- Ajustar según los valores que manejes

IF @PlanId IS NULL
BEGIN
SELECT 'SubscriptionExpiredOrInactive' AS AccessStatus;
RETURN;
END

    -- 3. Si se solicita un módulo específico, validar que esté en el plan
    IF @module_name IS NOT NULL
BEGIN
        IF NOT EXISTS (
            SELECT 1 
            FROM [config].[PlanModule] pm WITH (NOLOCK) 
            INNER JOIN [config].[Module] m WITH (NOLOCK) ON pm.ModuleId = m.Id
            WHERE pm.MembershipPlanId = @PlanId 
              AND (m.Code = @module_name OR m.Name = @module_name)
              AND m.Active = 1
        )
BEGIN
SELECT 'ModuleNotIncluded' AS AccessStatus;
RETURN;
END
END

    -- 4. Acceso concedido
SELECT 'Success' AS AccessStatus;
END
GO
CREATE OR ALTER PROCEDURE [auth].[sp_login_user]
(
    @username VARCHAR(150),
    @company_id INT
)
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Validar que la compañía existe y tiene habilitado el acceso
    IF NOT EXISTS (
        SELECT 1 
        FROM [config].[ApplicationCompanies] WITH (NOLOCK) 
        WHERE CompanyId = @company_id AND IsEnabled = 1 AND Active = 1
    )
BEGIN
        RAISERROR('La compañía no tiene una configuración de aplicación activa.', 16, 1);
        RETURN;
END

    -- 2. Búsqueda del usuario basada en la configuración de la empresa
    ;WITH CompanyConfig AS (
    SELECT LoginField, Priority
    FROM [config].[ApplicationCompanies] WITH (NOLOCK)
     WHERE CompanyId = @company_id AND IsEnabled = 1 AND Active = 1
         )
SELECT TOP 1
        ui.Id AS UserId,
    @username as UserName,
       ui.DocumentNumber,
       ui.FirstName,
       ui.LastName,
       ui.Email,
       ui.Phone,
       ui.Password,
       ui.RoleId,
       c.Name AS CompanyName,
       c.Id AS CompanyId
FROM [auth].[UsersInfo] ui WITH (NOLOCK)
    INNER JOIN [business].[Companies] c WITH (NOLOCK) ON ui.CompanyId = c.Id
    CROSS JOIN CompanyConfig cc
WHERE ui.CompanyId = @company_id
  AND ui.Active = 1
  AND c.Active = 1
  AND (
    (cc.LoginField = 'email' AND ui.Email = @username) OR
    (cc.LoginField = 'phone' AND ui.Phone = @username) OR
    (cc.LoginField = 'documentNumber' AND ui.DocumentNumber = @username)
    )
ORDER BY cc.Priority;

IF @@ROWCOUNT = 0
BEGIN
        RAISERROR('Usuario no encontrado o método de login no válido para esta compañía.', 16, 1);
END
END
GO
CREATE OR ALTER PROCEDURE [auth].[sp_get_user_by_email_company]
(
    @Email VARCHAR(255),
    @CompanyId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        ui.Id AS UserId,
        ui.Email as UserName,
        ui.Email,
        ui.FirstName,
        ui.LastName,
        ui.CompanyId
    FROM [auth].[UsersInfo] ui WITH (NOLOCK)
    WHERE ui.Email = @Email
      AND ui.CompanyId = @CompanyId
      AND ui.Active = 1;
END
GO

CREATE OR ALTER PROCEDURE [auth].[sp_insert_password_reset_token]
(
    @UserId INT,
    @Token VARCHAR(255),
    @ExpiresAt DATETIME2,
    @CreatedAt DATETIME2
)
AS
BEGIN
    SET NOCOUNT ON;

INSERT INTO [audit].[PasswordResetToken] (UserId, Token, ExpiresAt, CreatedAt, IsActive)
VALUES (@UserId, @Token, @ExpiresAt, @CreatedAt, 1);

SELECT SCOPE_IDENTITY() AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [auth].[sp_get_active_password_reset_token]
(
    @Token VARCHAR(255) = NULL,
    @UserId INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @UserId IS NOT NULL AND @UserId > 0 AND @Token IS NULL
BEGIN
SELECT pr.Id, pr.UserId, pr.Token, pr.ExpiresAt, pr.CreatedAt, pr.UsedAt,
       ui.Id AS UserId, ui.Email as UserName, ui.Email, ui.FirstName, ui.LastName, ui.CompanyId
FROM [audit].[PasswordResetToken] pr WITH (NOLOCK)
    INNER JOIN [auth].[UsersInfo] ui WITH (NOLOCK) ON pr.UserId = ui.Id
WHERE pr.UserId = @UserId
  AND pr.UsedAt IS NULL
  AND GETDATE() < pr.ExpiresAt
  AND pr.IsActive = 1;
END
ELSE
BEGIN
SELECT pr.Id, pr.UserId, pr.Token, pr.ExpiresAt, pr.CreatedAt, pr.UsedAt,
       ui.Id AS UserId, ui.Email as UserName, ui.Email, ui.FirstName, ui.LastName, ui.CompanyId
FROM [audit].[PasswordResetToken] pr WITH (NOLOCK)
    INNER JOIN [auth].[UsersInfo] ui WITH (NOLOCK) ON pr.UserId = ui.Id
WHERE pr.Token = @Token
  AND pr.UsedAt IS NULL
  AND GETDATE() < pr.ExpiresAt
  AND pr.IsActive = 1;
END

END
GO

CREATE OR ALTER PROCEDURE [auth].[sp_mark_password_reset_used]
(
    @Token VARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [audit].[PasswordResetToken]
    SET UsedAt = SYSUTCDATETIME(), IsActive = 0
    WHERE Token = @Token;
END
GO

CREATE OR ALTER PROCEDURE [auth].[sp_update_user_password]
(
    @UserId INT, 
    @NewPassword VARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

UPDATE [auth].[UsersInfo]
SET Password = @NewPassword
WHERE Id = @UserId;
END
GO
CREATE OR ALTER PROCEDURE [auth].[sp_list_users]
(
    @CompanyId INT,
    @Page INT,
    @PageSize INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@Page - 1) * @PageSize;

    SELECT
        ui.Id AS UserId,
        ui.CompanyId,
        ui.DocumentNumber,
        ui.FirstName,
        ui.LastName,
        ui.Email,
        ui.Phone,
        ui.RoleId,
        r.Name AS RoleName,
        ui.Active,
        ui.Created,
        ui.Updated
    FROM [auth].[UsersInfo] ui
    LEFT JOIN [config].[Role] r ON ui.RoleId = r.Id
    WHERE ui.CompanyId = @CompanyId
    ORDER BY ui.Id
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

CREATE OR ALTER PROCEDURE [auth].[sp_update_user]
(
    @UserId INT,
    @DocumentTypeId INT,
    @DocumentNumber VARCHAR(50),
    @FirstName VARCHAR(150),
    @LastName VARCHAR(150),
    @Email VARCHAR(150),
    @Phone VARCHAR(20),
    @PasswordHash NVARCHAR(255), -- NULL si no se actualiza
    @AuthProvider VARCHAR(50),
    @ExternalId VARCHAR(150),
    @RoleId INT,
    @Active BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [auth].[UsersInfo]
    SET DocumentTypeId = @DocumentTypeId,
        DocumentNumber = @DocumentNumber,
        FirstName = @FirstName,
        LastName = @LastName,
        Email = @Email,
        Phone = @Phone,
        AuthProvider = @AuthProvider,
        ExternalId = @ExternalId,
        RoleId = @RoleId,
        Active = @Active,
        Updated = SYSUTCDATETIME(),
        Password = CASE WHEN @PasswordHash IS NOT NULL THEN @PasswordHash ELSE Password END
    WHERE Id = @UserId;
END
GO
CREATE OR ALTER PROCEDURE [auth].[sp_get_user_by_id]
(
    @UserId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ui.Id AS UserId,
        ui.CompanyId,
        c.Name AS CompanyName,
        ui.DocumentNumber,
        ui.FirstName,
        ui.LastName,
        ui.Email,
        ui.Phone,
        ui.RoleId,
        r.Name AS RoleName,
        ui.ConfirmedAccount,
        ui.Active,
        ui.Created,
        ui.Updated
    FROM [auth].[UsersInfo] ui
    LEFT JOIN [business].[Companies] c ON ui.CompanyId = c.Id
    LEFT JOIN [config].[Role] r ON ui.RoleId = r.Id
    WHERE ui.Id = @UserId;
END
GO
CREATE OR ALTER PROCEDURE [auth].[sp_delete_user]
(
    @UserId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Soft delete
    UPDATE [auth].[UsersInfo]
    SET Active = 0, Updated = GETDATE()
    WHERE Id = @UserId;
END
GO
CREATE OR ALTER PROCEDURE [auth].[sp_create_user]
(
    @CompanyId INT,
    @DocumentTypeId INT,
    @DocumentNumber VARCHAR(50),
    @FirstName VARCHAR(150),
    @LastName VARCHAR(150),
    @Email VARCHAR(150),
    @Phone VARCHAR(20),
    @PasswordHash NVARCHAR(255),
    @AuthProvider VARCHAR(50),
    @ExternalId VARCHAR(150),
    @RoleId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [auth].[UsersInfo]
    (CompanyId, DocumentTypeId, DocumentNumber, FirstName, LastName, Email, Phone, Password, AuthProvider, ExternalId, RoleId, ConfirmedAccount, Active, Created, Updated)
    VALUES
    (@CompanyId, @DocumentTypeId, @DocumentNumber, @FirstName, @LastName, @Email, @Phone, @PasswordHash, @AuthProvider, @ExternalId, @RoleId, 0, 1, SYSUTCDATETIME(), SYSUTCDATETIME());

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS InsertedId;
END
GO

CREATE OR ALTER   PROCEDURE [auth].[sp_get_rol_user]
(
    @UserId INT
)
AS
BEGIN
    SET NOCOUNT ON;

SELECT
    ui.Id AS UserId,
    r.Name AS RoleName,
    r.IsSystemRole,
    r.IsSuperUser,
    ui.Active AS UserActive,
    r.Active AS RolActive
FROM [auth].[UsersInfo] ui WITH (NOLOCK)
    INNER JOIN [config].[Role] r WITH (NOLOCK) on ui.RoleId =r.Id
WHERE ui.Id = @UserId;
END
GO

-- ESQUEMA CONFIG
CREATE OR ALTER PROCEDURE [config].[sp_get_membership_plan_by_id]
    @Id INT
AS
BEGIN
SELECT Id, Name, Price, BillingCycle, DurationDays, MaxUsers, IsPublic
FROM [config].[MembershipPlan] WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_list_membership_plans]
AS
BEGIN
    SELECT Id, Name, Price, BillingCycle, DurationDays, MaxUsers, IsPublic
    FROM [config].[MembershipPlan];
END
GO

-- ESQUEMA COMMON
CREATE OR ALTER PROCEDURE [common].[sp_list_countries]
AS
BEGIN
    SELECT Id, Name, IsoCode, Active FROM [common].[Countries];
    END
GO

CREATE OR ALTER PROCEDURE [common].[sp_list_departments_by_country]
    @CountryId INT
AS
BEGIN
    SELECT
        d.Id,
        d.Name,
        d.CountryId,
        c.Name AS CountryName -- Nombre de la cascada
    FROM [common].[Department] d
        INNER JOIN [common].[Countries] c ON d.CountryId = c.Id
    WHERE d.CountryId = @CountryId;
END
GO

CREATE OR ALTER PROCEDURE [common].[sp_list_cities_by_department]
    @DepartmentId INT
AS
BEGIN
    SELECT
        ci.Id,
        ci.Name,
        ci.DepartmentId,
        de.Name AS DepartmentName, -- Nombre de la cascada 1
        de.CountryId,
        co.Name AS CountryName      -- Nombre de la cascada 2
    FROM [common].[City] ci
        INNER JOIN [common].[Department] de ON ci.DepartmentId = de.Id
        INNER JOIN [common].[Countries] co ON de.CountryId = co.Id
    WHERE ci.DepartmentId = @DepartmentId;
END
GO

CREATE OR ALTER PROCEDURE [common].[sp_get_city_by_id]
    @Id INT
AS
BEGIN
SELECT
    ci.Id,
    ci.Name,
    ci.DepartmentId,
    de.Name AS DepartmentName,
    de.CountryId,
    co.Name AS CountryName
FROM [common].[City] ci
    INNER JOIN [common].[Department] de ON ci.DepartmentId = de.Id
    INNER JOIN [common].[Countries] co ON de.CountryId = co.Id
WHERE ci.Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE [common].[sp_list_document_types]
AS
BEGIN
    SELECT Id, Name, Abbreviation, Active FROM [common].[DocumentType] WHERE Active = 1;
END
GO

CREATE OR ALTER PROCEDURE [common].[sp_get_document_type_by_id]
    @Id INT
AS
BEGIN
SELECT Id, Name, Abbreviation, Active FROM [common].[DocumentType] WHERE Id = @Id AND Active = 1;
END
GO

-- ESQUEMA AUDIT
CREATE OR ALTER PROCEDURE [audit].[sp_insert_user_session]
    @UserId INT,
    @LoginField VARCHAR(50),
    @CompanyId INT,
    @Platform VARCHAR(50),
    @DeviceFingerprint VARCHAR(255),
    @IpAddress VARCHAR(45),
    @UserAgent VARCHAR(500),
    @LoginAt DATETIME,
    @IsSuccess BIT,
    @FailureReason VARCHAR(100)
AS
BEGIN
INSERT INTO [audit].[UserSession] (UserId,LoginField, CompanyId, Platform, DeviceFingerprint, IpAddress, UserAgent, LoginAt, IsSuccess, FailureReason)
VALUES (@UserId, @LoginField, @CompanyId, @Platform, @DeviceFingerprint, @IpAddress, @UserAgent, @LoginAt, @IsSuccess, @FailureReason);

SELECT SCOPE_IDENTITY(); -- Retorna el Id (long) generado
END
GO

CREATE OR ALTER PROCEDURE [audit].[sp_update_user_session]
    @Id BIGINT,
    @LogoutAt DATETIME,
    @IsSuccess BIT,
    @FailureReason VARCHAR(100)
AS
BEGIN
UPDATE [audit].[UserSession]
SET LogoutAt = @LogoutAt,
    IsSuccess = @IsSuccess,
    FailureReason = @FailureReason
WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE [audit].[sp_get_user_session_by_id]
    @Id BIGINT
AS
BEGIN
SELECT * FROM [audit].[UserSession] WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE [audit].[sp_close_all_user_sessions]
    @UserId INT,
    @LogoutAt DATETIME
AS
BEGIN
UPDATE [audit].[UserSession]
SET LogoutAt = @LogoutAt,
    IsSuccess = 0
WHERE UserId = @UserId AND LogoutAt IS NULL AND IsSuccess = 1;
END
GO

CREATE OR ALTER PROCEDURE [audit].[sp_close_company_sessions]
    @CompanyId INT,
    @LogoutAt DATETIME
AS
BEGIN
UPDATE [audit].[UserSession]
SET LogoutAt = @LogoutAt,
    IsSuccess = 0
WHERE CompanyId = @CompanyId AND LogoutAt IS NULL AND IsSuccess = 1;
END
GO

-- ESQUEMA AUTH
CREATE OR ALTER PROCEDURE [auth].[sp_insert_refresh_token]
    @UserSessionId BIGINT,
    @Token VARCHAR(255),
    @ExpiresAt DATETIME,
    @CreatedAt DATETIME,
    @JwtId VARCHAR(50) = NULL,
    @AccessTokenExpiresAt DATETIME = NULL
AS
BEGIN
INSERT INTO [audit].[RefreshToken] (
    UserSessionId, Token, ExpiresAt, CreatedAt,
    RevokedAt, ReplacedByToken, JwtId, AccessTokenExpiresAt, Active
)
VALUES (
    @UserSessionId, @Token, @ExpiresAt, @CreatedAt,
    NULL, NULL, @JwtId, @AccessTokenExpiresAt, 1
    );

SELECT CAST(SCOPE_IDENTITY() AS BIGINT); -- Retorna BIGINT para mapear con long
END
GO

CREATE OR ALTER PROCEDURE [auth].[sp_get_active_refresh_token]
    @Token NVARCHAR(500), -- Cambiado a NVARCHAR para coincidir con tu C#
    @CurrentTime DATETIME
AS
BEGIN
    SET NOCOUNT ON;

SELECT
    -- Datos del Token
    rt.Token AS RefreshToken,
    rt.JwtId,
    rt.AccessTokenExpiresAt,
    rt.UserSessionId,

    -- Datos del Usuario con Lógica Dinámica
    u.Id AS UserId,
    u.DocumentNumber,
    u.FirstName,
    u.LastName,
    u.Email,
    u.Phone,
    u.Password,
    u.RoleId,
    c.Id AS CompanyId,
    c.Name AS CompanyName,
    us.LoginField,
    -- Si el campo LoginField dice 'phone', 'email' o 'document', 
    -- podrías querer devolver una columna específica de la tabla UsersInfo
    CASE
        WHEN us.LoginField = 'email' THEN u.Email
        WHEN us.LoginField = 'phone' THEN u.Phone -- Asumiendo que tienes esta columna
        WHEN us.LoginField = 'document' THEN u.DocumentNumber -- Asumiendo que tienes esta columna
        END AS UserName,

    -- Datos de la Empresa
    c.Name AS CompanyName
FROM [audit].[RefreshToken] rt WITH (NOLOCK)
    INNER JOIN [audit].[UserSession] us WITH (NOLOCK) ON rt.UserSessionId = us.Id
    INNER JOIN [auth].[UsersInfo] u WITH (NOLOCK) ON us.UserId = u.Id
    LEFT JOIN [business].[Companies] c WITH (NOLOCK) ON us.CompanyId = c.Id
WHERE rt.Token = @Token
  AND rt.RevokedAt IS NULL
  AND rt.ExpiresAt > @CurrentTime
  AND rt.Active = 1;
END
GO

CREATE OR ALTER PROCEDURE [auth].[sp_revoke_refresh_token]
    @Token VARCHAR(MAX),
    @RevokedAt DATETIME,
    @ReplacedByToken VARCHAR(MAX) = NULL
AS
BEGIN
UPDATE [audit].[RefreshToken]
SET RevokedAt = @RevokedAt,
    ReplacedByToken = @ReplacedByToken,
    Active = 0
WHERE Token = @Token;
END
GO

CREATE OR ALTER PROCEDURE [auth].[sp_revoke_all_tokens_by_user]
    @UserSessionId INT,
    @RevokedAt DATETIME,
    @CurrentTime DATETIME,
    @ReplacedByToken VARCHAR(MAX) = NULL
AS
BEGIN
UPDATE [audit].[RefreshToken]
SET RevokedAt = @RevokedAt,
    ReplacedByToken = @ReplacedByToken,
    Active = 0
WHERE UserSessionId = @UserSessionId
  AND RevokedAt IS NULL
  AND ExpiresAt > @CurrentTime
  And Active = 1;
END
GO

CREATE OR ALTER PROCEDURE [auth].[sp_revoke_all_tokens_by_company]
    @CompanyId INT,
    @RevokedAt DATETIME,
    @CurrentTime DATETIME,
    @ReplacedByToken NVARCHAR(500) = NULL -- Cambiado de MAX a 500 por rendimiento
AS
BEGIN
    SET NOCOUNT ON;

UPDATE rt
SET rt.RevokedAt = @RevokedAt,
    rt.ReplacedByToken = @ReplacedByToken,
    rt.Active = 0
    FROM [audit].[RefreshToken] rt with (NOLOCK)
    INNER JOIN [audit].[UserSession] us WITH (NOLOCK) ON rt.UserSessionId = us.Id
WHERE us.CompanyId = @CompanyId
  AND rt.RevokedAt IS NULL
  AND rt.ExpiresAt > @CurrentTime
  AND rt.Active = 1;
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_get_company_id_by_subdomain]
    @subdomain NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        c.Id AS CompanyId
    FROM [business].[Companies] c
    WHERE c.Active = 1
      AND LOWER(c.Subdomain) = LOWER(@subdomain);
END
GO
CREATE OR ALTER PROCEDURE [config].[sp_list_company_modules]
    @company_id INT
AS
BEGIN
    SET NOCOUNT ON;

    /*
      NOTE:
      Esta SP asume la existencia de la tabla [config].[CompanyModule].
      Si aún no existe, crea la tabla con el script correspondiente.
    */
    SELECT
        cm.CompanyId,
        cm.ModuleId,
        m.Code AS ModuleCode,
        m.Name AS ModuleName,
        cm.IsEnabled
    FROM [config].[CompanyModule] cm
    INNER JOIN [config].[Module] m ON m.Id = cm.ModuleId
    WHERE cm.CompanyId = @company_id
    ORDER BY m.Name;
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_is_company_module_enabled]
    @company_id INT,
    @module_code NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @module_id INT;
    SELECT TOP 1 @module_id = m.Id
    FROM [config].[Module] m
    WHERE LOWER(m.Code) = LOWER(@module_code)
      AND m.Active = 1;

    IF @module_id IS NULL
    BEGIN
        SELECT CAST(0 AS BIT) AS IsEnabled;
        RETURN;
    END

    -- Si no existe registro, por defecto NO habilitado (fail closed)
    IF EXISTS (
        SELECT 1
        FROM [config].[CompanyModule] cm
        WHERE cm.CompanyId = @company_id
          AND cm.ModuleId = @module_id
          AND cm.IsEnabled = 1
    )
        SELECT CAST(1 AS BIT) AS IsEnabled;
    ELSE
        SELECT CAST(0 AS BIT) AS IsEnabled;
END
GO
CREATE OR ALTER PROCEDURE [config].[sp_list_modules]
    @only_active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        m.Id,
        m.Code,
        m.Name,
        m.Description,
        m.Active
    FROM [config].[Module] m
    WHERE (@only_active = 0 OR m.Active = 1)
    ORDER BY m.Name;
END
GO
CREATE OR ALTER PROCEDURE [config].[sp_set_company_module]
    @company_id INT,
    @module_code NVARCHAR(50),
    @is_enabled BIT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @module_id INT;
    SELECT TOP 1 @module_id = m.Id
    FROM [config].[Module] m
    WHERE LOWER(m.Code) = LOWER(@module_code);

    IF @module_id IS NULL
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'Module not found' AS Message;
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM [config].[CompanyModule] WHERE CompanyId = @company_id AND ModuleId = @module_id)
    BEGIN
        UPDATE [config].[CompanyModule]
        SET IsEnabled = @is_enabled, UpdatedAt = SYSUTCDATETIME()
        WHERE CompanyId = @company_id AND ModuleId = @module_id;
    END
    ELSE
    BEGIN
        INSERT INTO [config].[CompanyModule] (CompanyId, ModuleId, IsEnabled, CreatedAt, UpdatedAt)
        VALUES (@company_id, @module_id, @is_enabled, SYSUTCDATETIME(), SYSUTCDATETIME());
    END

    SELECT CAST(1 AS BIT) AS Success, N'OK' AS Message;
END
GO
/*
  Tabla: CompanyModule
  Permite habilitar/deshabilitar módulos por compañía (además del plan).
*/

IF OBJECT_ID('[config].[CompanyModule]', 'U') IS NULL
BEGIN
    CREATE TABLE [config].[CompanyModule] (
        [CompanyId] INT NOT NULL,
        [ModuleId] INT NOT NULL,
        [IsEnabled] BIT NOT NULL CONSTRAINT DF_CompanyModule_IsEnabled DEFAULT (1),
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_CompanyModule_CreatedAt DEFAULT (SYSUTCDATETIME()),
        [UpdatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_CompanyModule_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_CompanyModule PRIMARY KEY ([CompanyId], [ModuleId]),
        CONSTRAINT FK_CompanyModule_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT FK_CompanyModule_Module FOREIGN KEY ([ModuleId]) REFERENCES [config].[Module]([Id])
    );
END
GO
/*
  HR (Workforce) - módulo común para finca/lavadero/comercio.
  Todo multi-tenant por CompanyId y referencia directa a auth.UsersInfo (UserId).
*/

-- Schema
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'hr')
    EXEC('CREATE SCHEMA [hr]');
GO

-- EmployeeProfile (1:1 con UsersInfo)
IF OBJECT_ID('[hr].[EmployeeProfile]', 'U') IS NULL
BEGIN
    CREATE TABLE [hr].[EmployeeProfile] (
        [UserId]            INT           NOT NULL,
        [CompanyId]         INT           NOT NULL,
        [IsEmployee]        BIT           NOT NULL CONSTRAINT DF_EmployeeProfile_IsEmployee DEFAULT (1),
        [ActiveEmployee]    BIT           NOT NULL CONSTRAINT DF_EmployeeProfile_ActiveEmployee DEFAULT (1),
        [HireDate]          DATE          NULL,

        -- Hospedaje
        [LodgingIncluded]   BIT           NOT NULL CONSTRAINT DF_EmployeeProfile_LodgingIncluded DEFAULT (0),
        [LodgingLocation]   NVARCHAR(200) NULL, -- cambuche/ubicación
        [MattressIncluded]  BIT           NOT NULL CONSTRAINT DF_EmployeeProfile_MattressIncluded DEFAULT (0),

        -- Alimentación parametrizable
        [MealPlanCode]      NVARCHAR(50)  NULL, -- FULL_DAY / ONE_MEAL / NONE
        [MealUnitCost]      DECIMAL(18,2) NULL, -- costo por unidad (si aplica)

        [CreatedAt]         DATETIME2(7)  NOT NULL CONSTRAINT DF_EmployeeProfile_CreatedAt DEFAULT (SYSUTCDATETIME()),
        [UpdatedAt]         DATETIME2(7)  NOT NULL CONSTRAINT DF_EmployeeProfile_UpdatedAt DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT PK_EmployeeProfile PRIMARY KEY ([UserId]),
        CONSTRAINT FK_EmployeeProfile_UsersInfo FOREIGN KEY ([UserId]) REFERENCES [auth].[UsersInfo]([Id]),
        CONSTRAINT FK_EmployeeProfile_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id])
    );
END
GO

-- PayScheme (cómo se paga)
IF OBJECT_ID('[hr].[PayScheme]', 'U') IS NULL
BEGIN
    CREATE TABLE [hr].[PayScheme] (
        [Id]        INT IDENTITY(1,1) NOT NULL,
        [CompanyId] INT NOT NULL,
        [Code]      NVARCHAR(50) NOT NULL,  -- DAILY / BY_WEIGHT / BY_SERVICE / FIXED
        [Name]      NVARCHAR(150) NOT NULL,
        [Unit]      NVARCHAR(50) NULL,      -- DAY / KG / SERVICE
        [Active]    BIT NOT NULL CONSTRAINT DF_PayScheme_Active DEFAULT (1),
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_PayScheme_CreatedAt DEFAULT (SYSUTCDATETIME()),
        [UpdatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_PayScheme_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_PayScheme PRIMARY KEY ([Id]),
        CONSTRAINT FK_PayScheme_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT UQ_PayScheme_Company_Code UNIQUE ([CompanyId], [Code])
    );
END
GO

-- PayRate (tarifa por esquema)
IF OBJECT_ID('[hr].[PayRate]', 'U') IS NULL
BEGIN
    CREATE TABLE [hr].[PayRate] (
        [Id]          INT IDENTITY(1,1) NOT NULL,
        [CompanyId]   INT NOT NULL,
        [PaySchemeId] INT NOT NULL,
        [Rate]        DECIMAL(18,2) NOT NULL, -- COP por unidad
        [EffectiveFrom] DATE NULL,
        [EffectiveTo]   DATE NULL,
        [Active]      BIT NOT NULL CONSTRAINT DF_PayRate_Active DEFAULT (1),
        [CreatedAt]   DATETIME2(7) NOT NULL CONSTRAINT DF_PayRate_CreatedAt DEFAULT (SYSUTCDATETIME()),
        [UpdatedAt]   DATETIME2(7) NOT NULL CONSTRAINT DF_PayRate_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_PayRate PRIMARY KEY ([Id]),
        CONSTRAINT FK_PayRate_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT FK_PayRate_PayScheme FOREIGN KEY ([PaySchemeId]) REFERENCES [hr].[PayScheme]([Id])
    );
END
GO

-- WorkLog (registro de trabajo que genera pago)
IF OBJECT_ID('[hr].[WorkLog]', 'U') IS NULL
BEGIN
    CREATE TABLE [hr].[WorkLog] (
        [Id]          BIGINT IDENTITY(1,1) NOT NULL,
        [CompanyId]   INT NOT NULL,
        [UserId]      INT NOT NULL,
        [WorkDate]    DATE NOT NULL,
        [PaySchemeId] INT NOT NULL,
        [Quantity]    DECIMAL(18,3) NOT NULL CONSTRAINT DF_WorkLog_Quantity DEFAULT (1),
        [Unit]        NVARCHAR(50) NULL,
        [ReferenceType] NVARCHAR(50) NULL,  -- ProductionBatch / ServiceOrder / POS / etc
        [ReferenceId]  NVARCHAR(50) NULL,
        [Notes]       NVARCHAR(500) NULL,
        [CreatedAt]   DATETIME2(7) NOT NULL CONSTRAINT DF_WorkLog_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_WorkLog PRIMARY KEY ([Id]),
        CONSTRAINT FK_WorkLog_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT FK_WorkLog_UsersInfo FOREIGN KEY ([UserId]) REFERENCES [auth].[UsersInfo]([Id]),
        CONSTRAINT FK_WorkLog_PayScheme FOREIGN KEY ([PaySchemeId]) REFERENCES [hr].[PayScheme]([Id])
    );
END
GO

-- LoanAdvance (anticipos/préstamos)
IF OBJECT_ID('[hr].[LoanAdvance]', 'U') IS NULL
BEGIN
    CREATE TABLE [hr].[LoanAdvance] (
        [Id]        BIGINT IDENTITY(1,1) NOT NULL,
        [CompanyId] INT NOT NULL,
        [UserId]    INT NOT NULL,
        [Date]      DATE NOT NULL,
        [Amount]    DECIMAL(18,2) NOT NULL,
        [Notes]     NVARCHAR(500) NULL,
        [Status]    NVARCHAR(20) NOT NULL CONSTRAINT DF_LoanAdvance_Status DEFAULT ('open'), -- open/paid/cancelled
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_LoanAdvance_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_LoanAdvance PRIMARY KEY ([Id]),
        CONSTRAINT FK_LoanAdvance_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT FK_LoanAdvance_UsersInfo FOREIGN KEY ([UserId]) REFERENCES [auth].[UsersInfo]([Id])
    );
END
GO

-- Deduction (deducciones)
IF OBJECT_ID('[hr].[Deduction]', 'U') IS NULL
BEGIN
    CREATE TABLE [hr].[Deduction] (
        [Id]        BIGINT IDENTITY(1,1) NOT NULL,
        [CompanyId] INT NOT NULL,
        [UserId]    INT NOT NULL,
        [Date]      DATE NOT NULL,
        [Amount]    DECIMAL(18,2) NOT NULL,
        [Type]      NVARCHAR(50) NULL,  -- MEAL / LOAN / OTHER
        [Notes]     NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_Deduction_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_Deduction PRIMARY KEY ([Id]),
        CONSTRAINT FK_Deduction_Companies FOREIGN KEY ([CompanyId]) REFERENCES [business].[Companies]([Id]),
        CONSTRAINT FK_Deduction_UsersInfo FOREIGN KEY ([UserId]) REFERENCES [auth].[UsersInfo]([Id])
    );
END
GO

/* =========================
   Stored Procedures (CRUD)
   ========================= */

-- EmployeeProfile upsert
CREATE OR ALTER PROCEDURE [hr].[sp_upsert_employee_profile]
    @company_id INT,
    @user_id INT,
    @is_employee BIT,
    @active_employee BIT,
    @hire_date DATE = NULL,
    @lodging_included BIT,
    @lodging_location NVARCHAR(200) = NULL,
    @mattress_included BIT,
    @meal_plan_code NVARCHAR(50) = NULL,
    @meal_unit_cost DECIMAL(18,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [hr].[EmployeeProfile] WHERE UserId=@user_id AND CompanyId=@company_id)
    BEGIN
        UPDATE [hr].[EmployeeProfile]
        SET
            IsEmployee=@is_employee,
            ActiveEmployee=@active_employee,
            HireDate=@hire_date,
            LodgingIncluded=@lodging_included,
            LodgingLocation=@lodging_location,
            MattressIncluded=@mattress_included,
            MealPlanCode=@meal_plan_code,
            MealUnitCost=@meal_unit_cost,
            UpdatedAt=SYSUTCDATETIME()
        WHERE UserId=@user_id AND CompanyId=@company_id;
    END
    ELSE
    BEGIN
        INSERT INTO [hr].[EmployeeProfile]
        (UserId, CompanyId, IsEmployee, ActiveEmployee, HireDate, LodgingIncluded, LodgingLocation, MattressIncluded, MealPlanCode, MealUnitCost)
        VALUES
        (@user_id, @company_id, @is_employee, @active_employee, @hire_date, @lodging_included, @lodging_location, @mattress_included, @meal_plan_code, @meal_unit_cost);
    END

    SELECT CAST(1 AS BIT) AS Success;
END
GO

CREATE OR ALTER PROCEDURE [hr].[sp_get_employee_profile]
    @company_id INT,
    @user_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        ep.UserId,
        ep.CompanyId,
        ep.IsEmployee,
        ep.ActiveEmployee,
        ep.HireDate,
        ep.LodgingIncluded,
        ep.LodgingLocation,
        ep.MattressIncluded,
        ep.MealPlanCode,
        ep.MealUnitCost
    FROM [hr].[EmployeeProfile] ep
    WHERE ep.CompanyId=@company_id AND ep.UserId=@user_id;
END
GO

CREATE OR ALTER PROCEDURE [hr].[sp_list_employees]
    @company_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.Id AS UserId,
        u.CompanyId,
        u.FirstName,
        u.LastName,
        u.DocumentNumber,
        u.Phone,
        u.Email,
        u.RoleId,
        u.Active AS UserActive,
        ep.ActiveEmployee,
        ep.HireDate
    FROM [auth].[UsersInfo] u
    LEFT JOIN [hr].[EmployeeProfile] ep ON ep.UserId = u.Id AND ep.CompanyId = u.CompanyId
    WHERE u.CompanyId=@company_id
      AND (ep.IsEmployee = 1 OR ep.UserId IS NULL) -- lista todos, pero marca empleado si existe
    ORDER BY u.FirstName, u.LastName;
END
GO

-- PayScheme
CREATE OR ALTER PROCEDURE [hr].[sp_upsert_pay_scheme]
    @company_id INT,
    @code NVARCHAR(50),
    @name NVARCHAR(150),
    @unit NVARCHAR(50) = NULL,
    @active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [hr].[PayScheme] WHERE CompanyId=@company_id AND Code=@code)
    BEGIN
        UPDATE [hr].[PayScheme]
        SET Name=@name, Unit=@unit, Active=@active, UpdatedAt=SYSUTCDATETIME()
        WHERE CompanyId=@company_id AND Code=@code;
    END
    ELSE
    BEGIN
        INSERT INTO [hr].[PayScheme] (CompanyId, Code, Name, Unit, Active)
        VALUES (@company_id, @code, @name, @unit, @active);
    END

    SELECT CAST(1 AS BIT) AS Success;
END
GO

CREATE OR ALTER PROCEDURE [hr].[sp_list_pay_schemes]
    @company_id INT,
    @only_active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, CompanyId, Code, Name, Unit, Active
    FROM [hr].[PayScheme]
    WHERE CompanyId=@company_id
      AND (@only_active=0 OR Active=1)
    ORDER BY Name;
END
GO

-- WorkLog
CREATE OR ALTER PROCEDURE [hr].[sp_create_work_log]
    @company_id INT,
    @user_id INT,
    @work_date DATE,
    @pay_scheme_id INT,
    @quantity DECIMAL(18,3),
    @unit NVARCHAR(50) = NULL,
    @reference_type NVARCHAR(50) = NULL,
    @reference_id NVARCHAR(50) = NULL,
    @notes NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [hr].[WorkLog]
    (CompanyId, UserId, WorkDate, PaySchemeId, Quantity, Unit, ReferenceType, ReferenceId, Notes)
    VALUES
    (@company_id, @user_id, @work_date, @pay_scheme_id, @quantity, @unit, @reference_type, @reference_id, @notes);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [hr].[sp_list_work_logs]
    @company_id INT,
    @user_id INT = NULL,
    @from_date DATE = NULL,
    @to_date DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        wl.Id,
        wl.CompanyId,
        wl.UserId,
        wl.WorkDate,
        wl.PaySchemeId,
        ps.Code AS PaySchemeCode,
        wl.Quantity,
        wl.Unit,
        wl.ReferenceType,
        wl.ReferenceId,
        wl.Notes
    FROM [hr].[WorkLog] wl
    INNER JOIN [hr].[PayScheme] ps ON ps.Id = wl.PaySchemeId
    WHERE wl.CompanyId=@company_id
      AND (@user_id IS NULL OR wl.UserId=@user_id)
      AND (@from_date IS NULL OR wl.WorkDate >= @from_date)
      AND (@to_date IS NULL OR wl.WorkDate <= @to_date)
    ORDER BY wl.WorkDate DESC, wl.Id DESC;
END
GO

-- LoanAdvance
CREATE OR ALTER PROCEDURE [hr].[sp_create_loan_advance]
    @company_id INT,
    @user_id INT,
    @date DATE,
    @amount DECIMAL(18,2),
    @notes NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [hr].[LoanAdvance] (CompanyId, UserId, Date, Amount, Notes)
    VALUES (@company_id, @user_id, @date, @amount, @notes);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [hr].[sp_list_loan_advances]
    @company_id INT,
    @user_id INT = NULL,
    @status NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CompanyId, UserId, Date, Amount, Notes, Status
    FROM [hr].[LoanAdvance]
    WHERE CompanyId=@company_id
      AND (@user_id IS NULL OR UserId=@user_id)
      AND (@status IS NULL OR Status=@status)
    ORDER BY Date DESC, Id DESC;
END
GO

-- Deduction
CREATE OR ALTER PROCEDURE [hr].[sp_create_deduction]
    @company_id INT,
    @user_id INT,
    @date DATE,
    @amount DECIMAL(18,2),
    @type NVARCHAR(50) = NULL,
    @notes NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [hr].[Deduction] (CompanyId, UserId, Date, Amount, Type, Notes)
    VALUES (@company_id, @user_id, @date, @amount, @type, @notes);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [hr].[sp_list_deductions]
    @company_id INT,
    @user_id INT = NULL,
    @type NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CompanyId, UserId, Date, Amount, Type, Notes
    FROM [hr].[Deduction]
    WHERE CompanyId=@company_id
      AND (@user_id IS NULL OR UserId=@user_id)
      AND (@type IS NULL OR Type=@type)
    ORDER BY Date DESC, Id DESC;
END
GO