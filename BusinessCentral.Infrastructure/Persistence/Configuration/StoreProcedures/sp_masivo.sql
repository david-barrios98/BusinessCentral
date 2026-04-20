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