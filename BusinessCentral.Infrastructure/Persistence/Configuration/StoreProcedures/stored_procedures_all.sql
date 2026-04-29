/*
  BusinessCentral - Stored Procedures (consolidado)

  Importante:
  - Este archivo contiene SOLO CREATE OR ALTER PROCEDURE (más creación idempotente de esquemas).
  - La creación de tablas se maneja por migraciones (EF); sin migraciones, las tablas no existen.
  - Los catálogos/maestros se insertan por Data Seed (JSON embebidos).
*/

/* Esquemas usados por tablas y procedimientos (idempotente).
   Si ya aplicaste migraciones EF, estos IF no hacen nada.
   Si ejecutas solo este script contra una BD vacía, necesitas también tablas (dotnet ef database update). */
IF SCHEMA_ID(N'auth') IS NULL EXEC(N'CREATE SCHEMA [auth]');
IF SCHEMA_ID(N'audit') IS NULL EXEC(N'CREATE SCHEMA [audit]');
IF SCHEMA_ID(N'business') IS NULL EXEC(N'CREATE SCHEMA [business]');
IF SCHEMA_ID(N'common') IS NULL EXEC(N'CREATE SCHEMA [common]');
IF SCHEMA_ID(N'config') IS NULL EXEC(N'CREATE SCHEMA [config]');
IF SCHEMA_ID(N'fin') IS NULL EXEC(N'CREATE SCHEMA [fin]');
IF SCHEMA_ID(N'com') IS NULL EXEC(N'CREATE SCHEMA [com]');
IF SCHEMA_ID(N'hr') IS NULL EXEC(N'CREATE SCHEMA [hr]');
IF SCHEMA_ID(N'farm') IS NULL EXEC(N'CREATE SCHEMA [farm]');
IF SCHEMA_ID(N'svc') IS NULL EXEC(N'CREATE SCHEMA [svc]');
IF SCHEMA_ID(N'mfg') IS NULL EXEC(N'CREATE SCHEMA [mfg]');
IF SCHEMA_ID(N'agro') IS NULL EXEC(N'CREATE SCHEMA [agro]');
GO

SET NOCOUNT ON;
GO

/* =========================================================
   CONFIG
   ========================================================= */

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

CREATE OR ALTER PROCEDURE [config].[sp_check_tenant_access]
    @company_id INT,
    @module_name VARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CurrentDate DATETIME = GETUTCDATE();

    IF NOT EXISTS (SELECT 1 FROM [business].[Companies] WITH (NOLOCK) WHERE Id = @company_id AND Active = 1)
    BEGIN
        SELECT 'CompanyDisabled' AS AccessStatus;
        RETURN;
    END

    DECLARE @PlanId INT;
    SELECT @PlanId = MembershipPlanId
    FROM [config].[CompanySubscription] WITH (NOLOCK)
    WHERE CompanyId = @company_id
      AND @CurrentDate BETWEEN StartDate AND EndDate
      AND LOWER([Status]) = 'active';

    IF @PlanId IS NULL
    BEGIN
        SELECT 'SubscriptionExpiredOrInactive' AS AccessStatus;
        RETURN;
    END

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

    SELECT 'Success' AS AccessStatus;
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

CREATE OR ALTER PROCEDURE [config].[sp_list_permissions]
(
    @only_active BIT = 1,
    @module_code NVARCHAR(50) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.Id AS PermissionId,
        m.Id AS ModuleId,
        m.Code AS ModuleCode,
        m.Name AS ModuleName,
        p.Code AS PermissionCode,
        p.Name AS PermissionName,
        CAST(1 AS BIT) AS Active
    FROM [config].[Permission] p WITH (NOLOCK)
    INNER JOIN [config].[Module] m WITH (NOLOCK) ON m.Id = p.ModuleId
    WHERE (@module_code IS NULL OR LOWER(m.Code) = LOWER(@module_code))
      AND m.Active = 1
    ORDER BY m.Code, p.Code;
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_list_business_nature_default_permissions]
(
    @nature_code NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @nature_id INT;
    SELECT TOP 1 @nature_id = bn.Id
    FROM [config].[BusinessNature] bn WITH (NOLOCK)
    WHERE LOWER(bn.Code) = LOWER(@nature_code)
      AND bn.Active = 1;

    IF @nature_id IS NULL
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'Business nature not found' AS Message;
        RETURN;
    END

    -- Estándar: los permisos default de una naturaleza son TODOS los permisos de los módulos
    -- que están marcados como default-enabled en BusinessNatureModule.
    SELECT
        p.Id AS PermissionId,
        m.Code AS ModuleCode,
        p.Code AS PermissionCode,
        p.Name AS PermissionName
    FROM [config].[BusinessNatureModule] bnm WITH (NOLOCK)
    INNER JOIN [config].[Module] m WITH (NOLOCK) ON m.Id = bnm.ModuleId AND m.Active = 1
    INNER JOIN [config].[Permission] p WITH (NOLOCK) ON p.ModuleId = m.Id
    WHERE bnm.BusinessNatureId = @nature_id
      AND bnm.IsDefaultEnabled = 1
    ORDER BY m.Code, p.Code;
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_list_company_modules]
    @company_id INT
AS
BEGIN
    SET NOCOUNT ON;

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

CREATE OR ALTER PROCEDURE [config].[sp_list_role_permissions]
    @role_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.Id AS PermissionId,
        m.Code AS ModuleCode,
        p.Code AS PermissionCode,
        p.Name AS PermissionName
    FROM [config].[RolePermission] rp WITH (NOLOCK)
    INNER JOIN [config].[Permission] p WITH (NOLOCK) ON p.Id = rp.PermissionId
    INNER JOIN [config].[Module] m WITH (NOLOCK) ON m.Id = p.ModuleId
    WHERE rp.RoleId = @role_id
      AND m.Active = 1
    ORDER BY m.Code, p.Code;
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_set_role_permission]
(
    @role_id INT,
    @permission_id INT,
    @enabled BIT = 1
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @enabled = 1
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM [config].[RolePermission] WHERE RoleId = @role_id AND PermissionId = @permission_id)
            INSERT INTO [config].[RolePermission] (RoleId, PermissionId) VALUES (@role_id, @permission_id);
    END
    ELSE
    BEGIN
        DELETE FROM [config].[RolePermission] WHERE RoleId = @role_id AND PermissionId = @permission_id;
    END

    SELECT CAST(1 AS BIT) AS Success, N'OK' AS Message;
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_apply_business_nature_defaults_to_company]
(
    @company_id INT,
    @nature_code NVARCHAR(50),
    @role_id INT = NULL -- opcional: si se pasa, aplica permisos default al rol
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @now DATETIME2 = SYSUTCDATETIME();

    DECLARE @nature_id INT;
    SELECT TOP 1 @nature_id = bn.Id
    FROM [config].[BusinessNature] bn WITH (NOLOCK)
    WHERE LOWER(bn.Code) = LOWER(@nature_code)
      AND bn.Active = 1;

    IF @nature_id IS NULL
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'Business nature not found' AS Message;
        RETURN;
    END

    -- 1) Módulos: asegura filas en CompanyModule según plantilla de naturaleza
    INSERT INTO [config].[CompanyModule] (CompanyId, ModuleId, IsEnabled, CreatedAt, UpdatedAt)
    SELECT
        @company_id,
        bnm.ModuleId,
        bnm.IsDefaultEnabled,
        @now,
        @now
    FROM [config].[BusinessNatureModule] bnm WITH (NOLOCK)
    WHERE bnm.BusinessNatureId = @nature_id
      AND NOT EXISTS (
          SELECT 1 FROM [config].[CompanyModule] cm
          WHERE cm.CompanyId = @company_id AND cm.ModuleId = bnm.ModuleId
      );

    -- 2) Permisos default: TODOS los permisos de módulos default-enabled, si role_id fue provisto
    IF @role_id IS NOT NULL AND @role_id > 0
    BEGIN
        INSERT INTO [config].[RolePermission] (RoleId, PermissionId)
        SELECT
            @role_id,
            p.Id AS PermissionId
        FROM [config].[BusinessNatureModule] bnm WITH (NOLOCK)
        INNER JOIN [config].[Module] m WITH (NOLOCK) ON m.Id = bnm.ModuleId AND m.Active = 1
        INNER JOIN [config].[Permission] p WITH (NOLOCK) ON p.ModuleId = m.Id
        WHERE bnm.BusinessNatureId = @nature_id
          AND bnm.IsDefaultEnabled = 1
          AND NOT EXISTS (
              SELECT 1 FROM [config].[RolePermission] rp
              WHERE rp.RoleId = @role_id AND rp.PermissionId = p.Id
          );
    END

    SELECT CAST(1 AS BIT) AS Success, N'OK' AS Message;
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

CREATE OR ALTER PROCEDURE [config].[sp_get_membership_plan_by_id]
    @Id INT
AS
BEGIN
    SELECT Id, Name, Price, BillingCycle, DurationDays, MaxUsers, IsPublic
    FROM [config].[MembershipPlan]
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_list_membership_plans]
AS
BEGIN
    SELECT Id, Name, Price, BillingCycle, DurationDays, MaxUsers, IsPublic
    FROM [config].[MembershipPlan];
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_list_plan_modules]
    @MembershipPlanId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pm.ModuleId,
        m.Code AS ModuleCode,
        m.Name AS ModuleName
    FROM [config].[PlanModule] pm
    INNER JOIN [config].[Module] m ON m.Id = pm.ModuleId AND m.Active = 1
    WHERE pm.MembershipPlanId = @MembershipPlanId
    ORDER BY m.Name;
END
GO

/* Clientes de aplicación (APK, escritorio, web) — qué campo usar para login por compañía */
CREATE OR ALTER PROCEDURE [config].[sp_list_application_companies]
    @company_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ac.Id,
        ac.CompanyId,
        ac.ApplicationCode,
        ac.LoginField,
        ac.Priority,
        ac.IsEnabled,
        ac.[Create],
        ac.[Update],
        ac.Active
    FROM [config].[ApplicationCompanies] ac WITH (NOLOCK)
    WHERE ac.CompanyId = @company_id
    ORDER BY ac.ApplicationCode, ac.Priority, ac.Id;
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_upsert_application_company]
    @Id INT = NULL,
    @CompanyId INT,
    @ApplicationCode NVARCHAR(200),
    @LoginField NVARCHAR(20),
    @Priority INT,
    @IsEnabled BIT,
    @Active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @now DATETIME2 = SYSUTCDATETIME();

    IF @Id IS NOT NULL AND @Id > 0
    BEGIN
        UPDATE [config].[ApplicationCompanies]
        SET
            ApplicationCode = @ApplicationCode,
            LoginField = @LoginField,
            Priority = @Priority,
            IsEnabled = @IsEnabled,
            Active = @Active,
            [Update] = @now
        WHERE Id = @Id AND CompanyId = @CompanyId;

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('No se encontró el registro o no pertenece a la compañía.', 16, 1);
            RETURN;
        END

        SELECT @Id AS Id;
        RETURN;
    END

    INSERT INTO [config].[ApplicationCompanies]
        (CompanyId, ApplicationCode, LoginField, Priority, IsEnabled, [Create], [Update], Active)
    VALUES
        (@CompanyId, @ApplicationCode, @LoginField, @Priority, @IsEnabled, @now, @now, @Active);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_delete_application_company]
    @Id INT,
    @CompanyId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [config].[ApplicationCompanies]
    WHERE Id = @Id AND CompanyId = @CompanyId;
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_list_business_natures]
    @only_active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        bn.Id,
        bn.Code,
        bn.Name,
        bn.Description,
        bn.Active
    FROM [config].[BusinessNature] bn
    WHERE (@only_active = 0 OR bn.Active = 1)
    ORDER BY bn.Name;
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_list_business_nature_modules]
    @nature_code NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @nature_id INT;
    SELECT TOP 1 @nature_id = bn.Id
    FROM [config].[BusinessNature] bn
    WHERE LOWER(bn.Code) = LOWER(@nature_code)
      AND bn.Active = 1;

    IF @nature_id IS NULL
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'Business nature not found' AS Message;
        RETURN;
    END

    SELECT
        bnm.BusinessNatureId,
        m.Id AS ModuleId,
        m.Code AS ModuleCode,
        m.Name AS ModuleName,
        bnm.IsDefaultEnabled,
        bnm.SortOrder
    FROM [config].[BusinessNatureModule] bnm
    INNER JOIN [config].[Module] m ON m.Id = bnm.ModuleId
    WHERE bnm.BusinessNatureId = @nature_id
      AND m.Active = 1
    ORDER BY bnm.SortOrder, m.Name;
END
GO

/* =========================================================
   CONFIG: Fulfillment / métodos de entrega/recepción
   ========================================================= */

CREATE OR ALTER PROCEDURE [config].[sp_list_fulfillment_methods]
(
    @only_active BIT = 1
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        fm.Id,
        fm.Code,
        fm.Name,
        fm.AppliesTo,
        fm.[Description],
        fm.Active
    FROM [config].[FulfillmentMethod] fm WITH (NOLOCK)
    WHERE (@only_active = 0 OR fm.Active = 1)
    ORDER BY fm.Name;
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_list_company_fulfillment_methods]
(
    @company_id INT,
    @only_enabled BIT = 1
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        fm.Id,
        fm.Code,
        fm.Name,
        fm.AppliesTo,
        fm.[Description],
        fm.Active,
        COALESCE(cfm.IsEnabled, CAST(0 AS BIT)) AS IsEnabled
    FROM [config].[FulfillmentMethod] fm WITH (NOLOCK)
    LEFT JOIN [config].[CompanyFulfillmentMethod] cfm WITH (NOLOCK)
        ON cfm.FulfillmentMethodId = fm.Id AND cfm.CompanyId = @company_id
    WHERE fm.Active = 1
      AND (@only_enabled = 0 OR COALESCE(cfm.IsEnabled, CAST(0 AS BIT)) = 1)
    ORDER BY fm.Name;
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_set_company_fulfillment_method]
(
    @company_id INT,
    @method_code NVARCHAR(30),
    @enabled BIT = 1
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @method_id INT;
    SELECT TOP 1 @method_id = fm.Id
    FROM [config].[FulfillmentMethod] fm WITH (NOLOCK)
    WHERE fm.Active = 1 AND LOWER(fm.Code) = LOWER(@method_code);

    IF @method_id IS NULL
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'Method not found' AS Message;
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM [config].[CompanyFulfillmentMethod] WHERE CompanyId=@company_id AND FulfillmentMethodId=@method_id)
    BEGIN
        UPDATE [config].[CompanyFulfillmentMethod]
        SET IsEnabled=@enabled, UpdatedAt=SYSUTCDATETIME()
        WHERE CompanyId=@company_id AND FulfillmentMethodId=@method_id;
    END
    ELSE
    BEGIN
        INSERT INTO [config].[CompanyFulfillmentMethod] (CompanyId, FulfillmentMethodId, IsEnabled, CreatedAt, UpdatedAt)
        VALUES (@company_id, @method_id, @enabled, SYSUTCDATETIME(), SYSUTCDATETIME());
    END

    SELECT CAST(1 AS BIT) AS Success, N'OK' AS Message;
END
GO

/* =========================================================
   CONFIG: Payment methods
   ========================================================= */

CREATE OR ALTER PROCEDURE [config].[sp_list_payment_methods]
(
    @only_active BIT = 1
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pm.Id,
        pm.Code,
        pm.Name,
        pm.AppliesTo,
        pm.[Description],
        pm.Active
    FROM [config].[PaymentMethod] pm WITH (NOLOCK)
    WHERE (@only_active = 0 OR pm.Active = 1)
    ORDER BY pm.Name;
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_list_company_payment_methods]
(
    @company_id INT,
    @only_enabled BIT = 1
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pm.Id,
        pm.Code,
        pm.Name,
        pm.AppliesTo,
        pm.[Description],
        pm.Active,
        COALESCE(cpm.IsEnabled, CAST(0 AS BIT)) AS IsEnabled
    FROM [config].[PaymentMethod] pm WITH (NOLOCK)
    LEFT JOIN [config].[CompanyPaymentMethod] cpm WITH (NOLOCK)
        ON cpm.PaymentMethodId = pm.Id AND cpm.CompanyId = @company_id
    WHERE pm.Active = 1
      AND (@only_enabled = 0 OR COALESCE(cpm.IsEnabled, CAST(0 AS BIT)) = 1)
    ORDER BY pm.Name;
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_set_company_payment_method]
(
    @company_id INT,
    @method_code NVARCHAR(30),
    @enabled BIT = 1
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @method_id INT;
    SELECT TOP 1 @method_id = pm.Id
    FROM [config].[PaymentMethod] pm WITH (NOLOCK)
    WHERE pm.Active = 1 AND LOWER(pm.Code) = LOWER(@method_code);

    IF @method_id IS NULL
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'Method not found' AS Message;
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM [config].[CompanyPaymentMethod] WHERE CompanyId=@company_id AND PaymentMethodId=@method_id)
    BEGIN
        UPDATE [config].[CompanyPaymentMethod]
        SET IsEnabled=@enabled, UpdatedAt=SYSUTCDATETIME()
        WHERE CompanyId=@company_id AND PaymentMethodId=@method_id;
    END
    ELSE
    BEGIN
        INSERT INTO [config].[CompanyPaymentMethod] (CompanyId, PaymentMethodId, IsEnabled, CreatedAt, UpdatedAt)
        VALUES (@company_id, @method_id, @enabled, SYSUTCDATETIME(), SYSUTCDATETIME());
    END

    SELECT CAST(1 AS BIT) AS Success, N'OK' AS Message;
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_onboard_company]
(
    -- Company
    @CompanyName NVARCHAR(200),
    @TradeName NVARCHAR(200) = NULL,
    @Subdomain NVARCHAR(50) = NULL,
    @DocumentTypeId INT = NULL,
    @DocumentNumber NVARCHAR(50) = NULL,
    @Email NVARCHAR(150) = NULL,
    @Phone NVARCHAR(20) = NULL,
    @BusinessNatureCode NVARCHAR(50),

    -- Subscription
    @MembershipPlanId INT,
    @StartDate DATETIME2 = NULL,
    @AutoRenew BIT = 1,

    -- Main facility
    @FacilityTypeId INT,
    @FacilityName NVARCHAR(200),
    @FacilityCode NVARCHAR(200) = NULL,
    @FacilityEmail NVARCHAR(150) = NULL,
    @FacilityPhone NVARCHAR(20) = NULL,

    -- Owner user (SuperUser)
    @OwnerDocumentTypeId INT,
    @OwnerDocumentNumber NVARCHAR(50),
    @OwnerFirstName NVARCHAR(150),
    @OwnerLastName NVARCHAR(150),
    @OwnerEmail NVARCHAR(150),
    @OwnerPhone NVARCHAR(20),
    @OwnerPasswordHash NVARCHAR(255),
    @OwnerAuthProvider NVARCHAR(50) = 'local',
    @OwnerExternalId NVARCHAR(150) = NULL,
    @OwnerRoleId INT,

    -- Optional: JSON array of sedes (camelCase). Example:
    -- [{"facilityTypeId":1,"name":"Matriz","code":"HQ"},{"facilityTypeId":2,"name":"Tienda Norte"}]
    -- When non-empty valid JSON array, replaces single-facility parameters below.
    @FacilitiesJson NVARCHAR(MAX) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        DECLARE @now DATETIME2 = SYSUTCDATETIME();
        DECLARE @bn_id INT;
        SELECT TOP 1 @bn_id = bn.Id
        FROM [config].[BusinessNature] bn
        WHERE LOWER(bn.Code) = LOWER(@BusinessNatureCode)
          AND bn.Active = 1;

        IF @bn_id IS NULL
            THROW 50001, 'BusinessNature not found', 1;

        IF @StartDate IS NULL SET @StartDate = @now;

        IF @Subdomain IS NOT NULL AND EXISTS (
            SELECT 1 FROM [business].[Companies] c WHERE LOWER(c.Subdomain) = LOWER(@Subdomain) AND c.Active = 1
        )
            THROW 50002, 'Subdomain already exists', 1;

        INSERT INTO [business].[Companies]
            (Name, TradeName, DocumentTypeId, DocumentNumber, Email, Phone, Subdomain, BusinessNatureId, [Create], [Update], Active, FinancialBootstrapStatus)
        VALUES
            (@CompanyName, @TradeName, @DocumentTypeId, @DocumentNumber, @Email, @Phone, @Subdomain, @bn_id, @now, @now, 1, N'NOT_STARTED');

        DECLARE @company_id INT = CAST(SCOPE_IDENTITY() AS INT);

        -- Multi-naturaleza: registra la naturaleza primaria (permite agregar más después)
        INSERT INTO [config].[CompanyBusinessNature] (CompanyId, BusinessNatureId, IsPrimary, CreatedAt)
        VALUES (@company_id, @bn_id, 1, @now);

        -- Sedes: lista JSON o una sola sede (retrocompatible)
        DECLARE @use_facilities_json BIT = 0;
        IF (@FacilitiesJson IS NOT NULL AND LEN(LTRIM(RTRIM(@FacilitiesJson))) > 2 AND ISJSON(@FacilitiesJson) = 1)
        BEGIN
            IF EXISTS (SELECT 1 FROM OPENJSON(@FacilitiesJson))
                SET @use_facilities_json = 1;
        END

        IF @use_facilities_json = 1
        BEGIN
            ;WITH src AS (
                SELECT
                    j.FacilityTypeId,
                    j.Name,
                    j.Code,
                    j.Email,
                    j.Phone,
                    j.Priority,
                    ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
                FROM OPENJSON(@FacilitiesJson)
                WITH (
                    FacilityTypeId INT '$.facilityTypeId',
                    Name NVARCHAR(200) '$.name',
                    Code NVARCHAR(200) '$.code',
                    Email NVARCHAR(150) '$.email',
                    Phone NVARCHAR(20) '$.phone',
                    Priority INT '$.priority'
                ) AS j
                WHERE j.Name IS NOT NULL AND LTRIM(RTRIM(j.Name)) <> ''
                  AND j.FacilityTypeId IS NOT NULL AND j.FacilityTypeId > 0
            )
            INSERT INTO [business].[Facility]
                (CompanyId, FacilityTypeId, Name, Code, Email, Phone, Priority, [Create], [Update], Active)
            SELECT
                @company_id,
                FacilityTypeId,
                Name,
                Code,
                Email,
                Phone,
                COALESCE(Priority, rn),
                @now, @now, 1
            FROM src;
        END
        ELSE
        BEGIN
            INSERT INTO [business].[Facility]
                (CompanyId, FacilityTypeId, Name, Code, Email, Phone, Priority, [Create], [Update], Active)
            VALUES
                (@company_id, @FacilityTypeId, @FacilityName, @FacilityCode, @FacilityEmail, @FacilityPhone, 1, @now, @now, 1);
        END

        DECLARE @durationDays INT;
        SELECT @durationDays = DurationDays FROM [config].[MembershipPlan] WHERE Id = @MembershipPlanId;
        IF @durationDays IS NULL OR @durationDays <= 0
            THROW 50003, 'Invalid MembershipPlan', 1;

        DECLARE @endDate DATETIME2 = DATEADD(DAY, @durationDays, @StartDate);

        INSERT INTO [config].[CompanySubscription]
            (CompanyId, MembershipPlanId, StartDate, EndDate, [Status], AutoRenew)
        VALUES
            (@company_id, @MembershipPlanId, @StartDate, @endDate, 'active', @AutoRenew);

        -- Default login method for this company/application (API)
        INSERT INTO [config].[ApplicationCompanies]
            (CompanyId, ApplicationCode, LoginField, Priority, IsEnabled, [Create], [Update], Active)
        VALUES
            (@company_id, 'API', 'email', 1, 1, @now, @now, 1);

        -- Enable modules from nature template
        INSERT INTO [config].[CompanyModule] (CompanyId, ModuleId, IsEnabled, CreatedAt, UpdatedAt)
        SELECT
            @company_id,
            bnm.ModuleId,
            bnm.IsDefaultEnabled,
            @now,
            @now
        FROM [config].[BusinessNatureModule] bnm
        WHERE bnm.BusinessNatureId = @bn_id
          AND NOT EXISTS (
              SELECT 1 FROM [config].[CompanyModule] cm
              WHERE cm.CompanyId = @company_id AND cm.ModuleId = bnm.ModuleId
          );

        -- Membresía: alinear módulos habilitados con config.PlanModule (si el plan define filas)
        IF EXISTS (SELECT 1 FROM [config].[PlanModule] WHERE MembershipPlanId = @MembershipPlanId)
        BEGIN
            MERGE [config].[CompanyModule] AS cm
            USING (
                SELECT @company_id AS CompanyId, pm.ModuleId, CAST(1 AS BIT) AS IsEnabled
                FROM [config].[PlanModule] pm
                WHERE pm.MembershipPlanId = @MembershipPlanId
            ) AS src
            ON (cm.CompanyId = src.CompanyId AND cm.ModuleId = src.ModuleId)
            WHEN MATCHED THEN
                UPDATE SET IsEnabled = 1, UpdatedAt = @now
            WHEN NOT MATCHED BY TARGET THEN
                INSERT (CompanyId, ModuleId, IsEnabled, CreatedAt, UpdatedAt)
                VALUES (src.CompanyId, src.ModuleId, 1, @now, @now);

            UPDATE cm
            SET IsEnabled = 0, UpdatedAt = @now
            FROM [config].[CompanyModule] cm
            WHERE cm.CompanyId = @company_id
              AND cm.ModuleId NOT IN (
                  SELECT pm.ModuleId FROM [config].[PlanModule] pm WHERE pm.MembershipPlanId = @MembershipPlanId
              );
        END

        -- Enable fulfillment methods from nature template
        INSERT INTO [config].[CompanyFulfillmentMethod] (CompanyId, FulfillmentMethodId, IsEnabled, CreatedAt, UpdatedAt)
        SELECT
            @company_id,
            bnf.FulfillmentMethodId,
            bnf.IsDefaultEnabled,
            @now,
            @now
        FROM [config].[BusinessNatureFulfillmentMethod] bnf
        WHERE bnf.BusinessNatureId = @bn_id
          AND NOT EXISTS (
              SELECT 1 FROM [config].[CompanyFulfillmentMethod] cfm
              WHERE cfm.CompanyId = @company_id AND cfm.FulfillmentMethodId = bnf.FulfillmentMethodId
          );

        -- Enable payment methods from nature template
        INSERT INTO [config].[CompanyPaymentMethod] (CompanyId, PaymentMethodId, IsEnabled, CreatedAt, UpdatedAt)
        SELECT
            @company_id,
            bnp.PaymentMethodId,
            bnp.IsDefaultEnabled,
            @now,
            @now
        FROM [config].[BusinessNaturePaymentMethod] bnp
        WHERE bnp.BusinessNatureId = @bn_id
          AND NOT EXISTS (
              SELECT 1 FROM [config].[CompanyPaymentMethod] cpm
              WHERE cpm.CompanyId = @company_id AND cpm.PaymentMethodId = bnp.PaymentMethodId
          );

        -- Create owner user through existing SP
        DECLARE @InsertedOwner TABLE (InsertedId INT);
        INSERT INTO @InsertedOwner(InsertedId)
        EXEC [auth].[sp_create_user]
            @CompanyId = @company_id,
            @DocumentTypeId = @OwnerDocumentTypeId,
            @DocumentNumber = @OwnerDocumentNumber,
            @FirstName = @OwnerFirstName,
            @LastName = @OwnerLastName,
            @Email = @OwnerEmail,
            @Phone = @OwnerPhone,
            @PasswordHash = @OwnerPasswordHash,
            @AuthProvider = @OwnerAuthProvider,
            @ExternalId = @OwnerExternalId,
            @RoleId = @OwnerRoleId;

        DECLARE @owner_user_id INT;
        SELECT TOP 1 @owner_user_id = InsertedId FROM @InsertedOwner;

        COMMIT;

        SELECT
            CAST(1 AS BIT) AS Success,
            @company_id AS CompanyId,
            @owner_user_id AS OwnerUserId,
            @bn_id AS BusinessNatureId,
            @MembershipPlanId AS MembershipPlanId,
            @StartDate AS StartDate,
            @endDate AS EndDate;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@msg, 16, 1);
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_list_company_business_natures]
(
    @company_id INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        cbn.CompanyId,
        cbn.BusinessNatureId,
        bn.Code AS NatureCode,
        bn.Name AS NatureName,
        cbn.IsPrimary,
        cbn.CreatedAt
    FROM [config].[CompanyBusinessNature] cbn WITH (NOLOCK)
    INNER JOIN [config].[BusinessNature] bn WITH (NOLOCK) ON bn.Id = cbn.BusinessNatureId
    WHERE cbn.CompanyId = @company_id
    ORDER BY cbn.IsPrimary DESC, bn.Name;
END
GO

CREATE OR ALTER PROCEDURE [config].[sp_set_company_business_nature]
(
    @company_id INT,
    @nature_code NVARCHAR(50),
    @is_primary BIT = 0,
    @enabled BIT = 1
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @bn_id INT;
    SELECT TOP 1 @bn_id = bn.Id
    FROM [config].[BusinessNature] bn WITH (NOLOCK)
    WHERE LOWER(bn.Code) = LOWER(@nature_code) AND bn.Active = 1;

    IF @bn_id IS NULL
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'BusinessNature not found' AS Message;
        RETURN;
    END

    IF @enabled = 0
    BEGIN
        -- No permitimos borrar si es la única naturaleza o si es primaria
        IF EXISTS (SELECT 1 FROM [config].[CompanyBusinessNature] WHERE CompanyId = @company_id AND BusinessNatureId = @bn_id AND IsPrimary = 1)
        BEGIN
            SELECT CAST(0 AS BIT) AS Success, N'Cannot remove primary nature' AS Message;
            RETURN;
        END

        DELETE FROM [config].[CompanyBusinessNature]
        WHERE CompanyId = @company_id AND BusinessNatureId = @bn_id;

        SELECT CAST(1 AS BIT) AS Success, N'OK' AS Message;
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM [config].[CompanyBusinessNature] WHERE CompanyId = @company_id AND BusinessNatureId = @bn_id)
    BEGIN
        INSERT INTO [config].[CompanyBusinessNature] (CompanyId, BusinessNatureId, IsPrimary, CreatedAt)
        VALUES (@company_id, @bn_id, @is_primary, SYSUTCDATETIME());
    END
    ELSE
    BEGIN
        UPDATE [config].[CompanyBusinessNature]
        SET IsPrimary = @is_primary
        WHERE CompanyId = @company_id AND BusinessNatureId = @bn_id;
    END

    -- Si se marca primaria, desmarca otras y mantiene Companies.BusinessNatureId sincronizado.
    IF @is_primary = 1
    BEGIN
        UPDATE [config].[CompanyBusinessNature]
        SET IsPrimary = 0
        WHERE CompanyId = @company_id AND BusinessNatureId <> @bn_id;

        UPDATE [business].[Companies]
        SET BusinessNatureId = @bn_id, [Update] = SYSUTCDATETIME()
        WHERE Id = @company_id;
    END

    -- Aplicar estándares por naturaleza: asegura CompanyModule + (opcional) permisos default.
    -- Nota: el rol a usar para defaults depende de tu estrategia; aquí no asumimos uno.
    EXEC [config].[sp_apply_business_nature_defaults_to_company]
        @company_id = @company_id,
        @nature_code = @nature_code,
        @role_id = NULL;

    SELECT CAST(1 AS BIT) AS Success, N'OK' AS Message;
END
GO

/* =========================================================
   COMMON
   ========================================================= */

CREATE OR ALTER PROCEDURE [common].[sp_list_countries]
AS
BEGIN
    SELECT Id, Name, IsoCode, Active FROM [common].[Countries];
END
GO

/* =========================================================
   FIN (Reportes financieros/contables + base tributaria CO)
   ========================================================= */

CREATE OR ALTER PROCEDURE [fin].[sp_create_financial_transaction]
(
    @company_id INT,
    @tx_date DATETIME2,
    @direction NVARCHAR(20),
    @kind NVARCHAR(30),
    @category_code NVARCHAR(50) = NULL,
    @description NVARCHAR(500) = NULL,
    @amount DECIMAL(18,2),
    @third_party_document NVARCHAR(50) = NULL,
    @third_party_name NVARCHAR(200) = NULL,
    @source_module NVARCHAR(50) = NULL,
    @reference_type NVARCHAR(50) = NULL,
    @reference_id NVARCHAR(100) = NULL,
    @tax_code NVARCHAR(50) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [fin].[FinancialTransaction]
        (CompanyId, TxDate, Direction, Kind, CategoryCode, [Description], Amount, ThirdPartyDocument, ThirdPartyName, SourceModule, ReferenceType, ReferenceId, TaxCode, Active, CreatedAt, UpdatedAt)
    VALUES
        (@company_id, @tx_date, @direction, @kind, @category_code, @description, @amount, @third_party_document, @third_party_name, @source_module, @reference_type, @reference_id, @tax_code, 1, SYSUTCDATETIME(), SYSUTCDATETIME());

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [fin].[sp_list_financial_transactions]
(
    @company_id INT,
    @from_date DATETIME2 = NULL,
    @to_date DATETIME2 = NULL,
    @page INT = 1,
    @page_size INT = 50
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@page - 1) * @page_size;

    ;WITH T AS (
        SELECT
            t.Id,
            t.CompanyId,
            t.TxDate,
            t.Direction,
            t.Kind,
            t.CategoryCode,
            t.[Description],
            t.Amount,
            t.ThirdPartyDocument,
            t.ThirdPartyName,
            t.SourceModule,
            t.ReferenceType,
            t.ReferenceId,
            t.TaxCode,
            t.Active,
            t.CreatedAt,
            t.UpdatedAt
        FROM [fin].[FinancialTransaction] t
        WHERE t.CompanyId = @company_id
          AND t.Active = 1
          AND (@from_date IS NULL OR t.TxDate >= @from_date)
          AND (@to_date IS NULL OR t.TxDate < DATEADD(DAY, 1, CAST(@to_date AS DATE)))
    )
    SELECT Id, CompanyId, TxDate, Direction, Kind, CategoryCode, [Description], Amount,
        ThirdPartyDocument, ThirdPartyName, SourceModule, ReferenceType, ReferenceId, TaxCode, Active, CreatedAt, UpdatedAt
    FROM T
    ORDER BY TxDate DESC, Id DESC
    OFFSET @Offset ROWS FETCH NEXT @page_size ROWS ONLY;

    SELECT COUNT(1) AS Total
    FROM [fin].[FinancialTransaction] t
    WHERE t.CompanyId = @company_id
      AND t.Active = 1
      AND (@from_date IS NULL OR t.TxDate >= @from_date)
      AND (@to_date IS NULL OR t.TxDate < DATEADD(DAY, 1, CAST(@to_date AS DATE)));
END
GO

CREATE OR ALTER PROCEDURE [fin].[sp_report_financial_summary]
(
    @company_id INT,
    @from_date DATETIME2,
    @to_date DATETIME2
)
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH Tx AS (
        SELECT
            CAST(t.TxDate AS DATE) AS [Day],
            CASE WHEN t.Direction = 'IN' THEN t.Amount ELSE 0 END AS InAmount,
            CASE WHEN t.Direction = 'OUT' THEN t.Amount ELSE 0 END AS OutAmount
        FROM [fin].[FinancialTransaction] t WITH (NOLOCK)
        WHERE t.CompanyId = @company_id
          AND t.Active = 1
          AND t.TxDate >= @from_date AND t.TxDate < DATEADD(DAY, 1, @to_date)
    ),
    Pos AS (
        SELECT
            CAST(p.PaidAt AS DATE) AS [Day],
            SUM(p.Amount) AS InAmount
        FROM [com].[PosPayment] p WITH (NOLOCK)
        INNER JOIN [com].[PosTicket] tk WITH (NOLOCK) ON tk.Id = p.TicketId AND tk.CompanyId = p.CompanyId
        WHERE p.CompanyId = @company_id
          AND p.PaidAt >= @from_date AND p.PaidAt < DATEADD(DAY, 1, @to_date)
          AND LOWER(tk.Status) IN ('paid','closed')
        GROUP BY CAST(p.PaidAt AS DATE)
    )
    SELECT
        d.[Day],
        SUM(d.InAmount) AS InAmount,
        SUM(d.OutAmount) AS OutAmount,
        SUM(d.InAmount) - SUM(d.OutAmount) AS Net
    FROM (
        SELECT [Day], InAmount, OutAmount FROM Tx
        UNION ALL
        SELECT [Day], InAmount, CAST(0 AS DECIMAL(18,2)) AS OutAmount FROM Pos
    ) d
    GROUP BY d.[Day]
    ORDER BY d.[Day];
END
GO

CREATE OR ALTER PROCEDURE [fin].[sp_report_pnl]
(
    @company_id INT,
    @from_date DATETIME2,
    @to_date DATETIME2
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Solo transacciones clasificadas (CategoryCode). POS se clasifica como SALES.
    ;WITH Tx AS (
        SELECT
            ISNULL(NULLIF(LTRIM(RTRIM(t.CategoryCode)), ''), 'UNCLASSIFIED') AS CategoryCode,
            SUM(CASE WHEN t.Direction = 'IN' THEN t.Amount ELSE 0 END) AS Income,
            SUM(CASE WHEN t.Direction = 'OUT' THEN t.Amount ELSE 0 END) AS Expense
        FROM [fin].[FinancialTransaction] t WITH (NOLOCK)
        WHERE t.CompanyId = @company_id
          AND t.Active = 1
          AND t.TxDate >= @from_date AND t.TxDate < DATEADD(DAY, 1, @to_date)
        GROUP BY ISNULL(NULLIF(LTRIM(RTRIM(t.CategoryCode)), ''), 'UNCLASSIFIED')
    ),
    Pos AS (
        SELECT
            'SALES_POS' AS CategoryCode,
            SUM(p.Amount) AS Income,
            CAST(0 AS DECIMAL(18,2)) AS Expense
        FROM [com].[PosPayment] p WITH (NOLOCK)
        INNER JOIN [com].[PosTicket] tk WITH (NOLOCK) ON tk.Id = p.TicketId AND tk.CompanyId = p.CompanyId
        WHERE p.CompanyId = @company_id
          AND p.PaidAt >= @from_date AND p.PaidAt < DATEADD(DAY, 1, @to_date)
          AND LOWER(tk.Status) IN ('paid','closed')
    )
    SELECT
        x.CategoryCode,
        SUM(x.Income) AS Income,
        SUM(x.Expense) AS Expense,
        SUM(x.Income) - SUM(x.Expense) AS Profit
    FROM (
        SELECT CategoryCode, Income, Expense FROM Tx
        UNION ALL
        SELECT CategoryCode, Income, Expense FROM Pos
    ) x
    GROUP BY x.CategoryCode
    ORDER BY x.CategoryCode;
END
GO

CREATE OR ALTER PROCEDURE [fin].[sp_report_tax_summary_co]
(
    @company_id INT,
    @from_date DATETIME2,
    @to_date DATETIME2
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Resumen tributario base (CO): suma de transacciones Kind='TAX' por TaxCode.
    SELECT
        ISNULL(NULLIF(LTRIM(RTRIM(t.TaxCode)), ''), 'UNSPECIFIED') AS TaxCode,
        SUM(CASE WHEN t.Direction = 'IN' THEN t.Amount ELSE 0 END) AS TaxIn,
        SUM(CASE WHEN t.Direction = 'OUT' THEN t.Amount ELSE 0 END) AS TaxOut,
        SUM(CASE WHEN t.Direction = 'OUT' THEN t.Amount ELSE 0 END) - SUM(CASE WHEN t.Direction = 'IN' THEN t.Amount ELSE 0 END) AS NetPayable
    FROM [fin].[FinancialTransaction] t WITH (NOLOCK)
    WHERE t.CompanyId = @company_id
      AND t.Active = 1
      AND t.Kind = 'TAX'
      AND t.TxDate >= @from_date AND t.TxDate < DATEADD(DAY, 1, @to_date)
    GROUP BY ISNULL(NULLIF(LTRIM(RTRIM(t.TaxCode)), ''), 'UNSPECIFIED')
    ORDER BY TaxCode;
END
GO

CREATE OR ALTER PROCEDURE [fin].[sp_report_renta_annual_co]
(
    @company_id INT,
    @year INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @from_date DATETIME2 = DATEFROMPARTS(@year, 1, 1);
    DECLARE @to_date DATETIME2 = DATEFROMPARTS(@year, 12, 31);

    -- Base para declaración de renta: ingresos, costos, gastos, impuestos (según CategoryCode/Kind).
    SELECT
        @year AS [Year],
        SUM(CASE WHEN t.Direction = 'IN' AND t.Kind <> 'TAX' THEN t.Amount ELSE 0 END) AS TotalIncome,
        SUM(CASE WHEN t.Direction = 'OUT' AND t.Kind <> 'TAX' THEN t.Amount ELSE 0 END) AS TotalExpense,
        SUM(CASE WHEN t.Kind = 'TAX' AND t.Direction = 'OUT' THEN t.Amount ELSE 0 END) AS TaxesPaid,
        SUM(CASE WHEN t.Kind = 'TAX' AND t.Direction = 'IN' THEN t.Amount ELSE 0 END) AS TaxesCollected,
        (SUM(CASE WHEN t.Direction = 'IN' AND t.Kind <> 'TAX' THEN t.Amount ELSE 0 END)
         - SUM(CASE WHEN t.Direction = 'OUT' AND t.Kind <> 'TAX' THEN t.Amount ELSE 0 END)) AS NetIncome
    FROM [fin].[FinancialTransaction] t WITH (NOLOCK)
    WHERE t.CompanyId = @company_id
      AND t.Active = 1
      AND t.TxDate >= @from_date AND t.TxDate < DATEADD(DAY, 1, @to_date);
END
GO

CREATE OR ALTER PROCEDURE [fin].[sp_list_accounts]
(
    @company_id INT,
    @only_active BIT = 1,
    @q NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.Id,
        a.CompanyId,
        a.Code,
        a.Name,
        a.Nature,
        a.Level,
        a.ParentAccountId,
        a.IsAuxiliary,
        a.Active
    FROM [fin].[Account] a WITH (NOLOCK)
    WHERE a.CompanyId = @company_id
      AND (@only_active = 0 OR a.Active = 1)
      AND (
          @q IS NULL
          OR a.Code LIKE '%' + @q + '%'
          OR a.Name LIKE '%' + @q + '%'
      )
    ORDER BY a.Code;
END
GO

CREATE OR ALTER PROCEDURE [fin].[sp_get_account_id_by_code]
(
    @company_id INT,
    @account_code NVARCHAR(20)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        a.Id
    FROM [fin].[Account] a WITH (NOLOCK)
    WHERE a.CompanyId = @company_id
      AND a.Active = 1
      AND a.Code = @account_code;
END
GO

CREATE OR ALTER PROCEDURE [business].[sp_list_facility_types]
    @only_active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ft.Id,
        ft.Name,
        ft.Active,
        ft.[Create],
        ft.[Update]
    FROM [business].[FacilityType] ft WITH (NOLOCK)
    WHERE (@only_active = 0 OR COALESCE(ft.Active, 1) = 1)
    ORDER BY ft.Name;
END
GO

/* --- Perfil de arranque financiero (empresa) --- */
CREATE OR ALTER PROCEDURE [business].[sp_get_company_financial_bootstrap]
    @company_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.Id AS CompanyId,
        c.FinancialStartupMode,
        c.FinancialOperatingStartDateUtc,
        c.FinancialBootstrapStatus,
        c.FinancialBootstrapNotes
    FROM [business].[Companies] c WITH (NOLOCK)
    WHERE c.Id = @company_id;
END
GO

CREATE OR ALTER PROCEDURE [business].[sp_update_company_financial_bootstrap]
(
    @company_id INT,
    @startup_mode NVARCHAR(30) = NULL,
    @operating_start_utc DATETIME2 = NULL,
    @bootstrap_status NVARCHAR(20),
    @notes NVARCHAR(500) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    /* NULL en modo/fecha/notas = no sobrescribir (permite updates parciales desde otros procesos). */
    UPDATE [business].[Companies]
    SET FinancialStartupMode = CASE WHEN @startup_mode IS NOT NULL THEN @startup_mode ELSE FinancialStartupMode END,
        FinancialOperatingStartDateUtc = CASE WHEN @operating_start_utc IS NOT NULL THEN @operating_start_utc ELSE FinancialOperatingStartDateUtc END,
        FinancialBootstrapStatus = @bootstrap_status,
        FinancialBootstrapNotes = CASE WHEN @notes IS NOT NULL THEN @notes ELSE FinancialBootstrapNotes END,
        [Update] = SYSUTCDATETIME()
    WHERE Id = @company_id;

    SELECT CAST(@@ROWCOUNT AS INT) AS RowsAffected;
END
GO

CREATE OR ALTER PROCEDURE [fin].[sp_create_journal_entry]
(
    @company_id INT,
    @entry_date DATETIME2,
    @entry_type NVARCHAR(30) = NULL,
    @reference_type NVARCHAR(50) = NULL,
    @reference_id NVARCHAR(100) = NULL,
    @description NVARCHAR(500) = NULL,
    @created_by_user_id INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [fin].[JournalEntry]
        (CompanyId, EntryDate, EntryType, ReferenceType, ReferenceId, [Description], [Status], CreatedByUserId, CreatedAt, UpdatedAt)
    VALUES
        (@company_id, @entry_date, @entry_type, @reference_type, @reference_id, @description, 'draft', @created_by_user_id, SYSUTCDATETIME(), SYSUTCDATETIME());

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [fin].[sp_add_journal_entry_line]
(
    @company_id INT,
    @journal_entry_id BIGINT,
    @account_id BIGINT,
    @debit DECIMAL(18,2),
    @credit DECIMAL(18,2),
    @third_party_document NVARCHAR(50) = NULL,
    @third_party_name NVARCHAR(200) = NULL,
    @notes NVARCHAR(500) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1 FROM [fin].[JournalEntry] je WITH (NOLOCK)
        WHERE je.Id = @journal_entry_id AND je.CompanyId = @company_id AND LOWER(je.[Status]) = 'draft'
    )
    BEGIN
        RAISERROR('JournalEntry not found or not in draft status.', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (
        SELECT 1 FROM [fin].[Account] a WITH (NOLOCK)
        WHERE a.Id = @account_id AND a.CompanyId = @company_id AND a.Active = 1
    )
    BEGIN
        RAISERROR('Account not found or inactive.', 16, 1);
        RETURN;
    END

    INSERT INTO [fin].[JournalEntryLine]
        (CompanyId, JournalEntryId, AccountId, Debit, Credit, ThirdPartyDocument, ThirdPartyName, Notes, CreatedAt)
    VALUES
        (@company_id, @journal_entry_id, @account_id, @debit, @credit, @third_party_document, @third_party_name, @notes, SYSUTCDATETIME());

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [fin].[sp_get_journal_entry]
(
    @company_id INT,
    @journal_entry_id BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        je.Id,
        je.CompanyId,
        je.EntryDate,
        je.EntryType,
        je.ReferenceType,
        je.ReferenceId,
        je.[Description],
        je.[Status],
        je.CreatedByUserId,
        je.CreatedAt,
        je.UpdatedAt
    FROM [fin].[JournalEntry] je WITH (NOLOCK)
    WHERE je.CompanyId = @company_id AND je.Id = @journal_entry_id;

    SELECT
        l.Id,
        l.JournalEntryId,
        l.AccountId,
        a.Code AS AccountCode,
        a.Name AS AccountName,
        l.Debit,
        l.Credit,
        l.ThirdPartyDocument,
        l.ThirdPartyName,
        l.Notes,
        l.CreatedAt
    FROM [fin].[JournalEntryLine] l WITH (NOLOCK)
    INNER JOIN [fin].[Account] a WITH (NOLOCK) ON a.Id = l.AccountId AND a.CompanyId = l.CompanyId
    WHERE l.CompanyId = @company_id AND l.JournalEntryId = @journal_entry_id
    ORDER BY l.Id;
END
GO

CREATE OR ALTER PROCEDURE [fin].[sp_post_journal_entry]
(
    @company_id INT,
    @journal_entry_id BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @debit DECIMAL(18,2) = 0;
    DECLARE @credit DECIMAL(18,2) = 0;

    SELECT
        @debit = ISNULL(SUM(Debit),0),
        @credit = ISNULL(SUM(Credit),0)
    FROM [fin].[JournalEntryLine] l WITH (NOLOCK)
    WHERE l.CompanyId = @company_id AND l.JournalEntryId = @journal_entry_id;

    IF @debit = 0 AND @credit = 0
    BEGIN
        RAISERROR('JournalEntry has no lines.', 16, 1);
        RETURN;
    END

    IF ABS(@debit - @credit) > 0.009
    BEGIN
        RAISERROR('JournalEntry not balanced (debit != credit).', 16, 1);
        RETURN;
    END

    UPDATE [fin].[JournalEntry]
    SET [Status] = 'posted', UpdatedAt = SYSUTCDATETIME()
    WHERE CompanyId = @company_id AND Id = @journal_entry_id AND LOWER([Status]) = 'draft';

    SELECT CAST(1 AS BIT) AS Success;
END
GO

CREATE OR ALTER PROCEDURE [fin].[sp_report_trial_balance]
(
    @company_id INT,
    @from_date DATETIME2,
    @to_date DATETIME2
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.Code AS AccountCode,
        a.Name AS AccountName,
        SUM(l.Debit) AS Debit,
        SUM(l.Credit) AS Credit,
        SUM(l.Debit - l.Credit) AS Balance
    FROM [fin].[JournalEntryLine] l WITH (NOLOCK)
    INNER JOIN [fin].[JournalEntry] je WITH (NOLOCK) ON je.Id = l.JournalEntryId AND je.CompanyId = l.CompanyId
    INNER JOIN [fin].[Account] a WITH (NOLOCK) ON a.Id = l.AccountId AND a.CompanyId = l.CompanyId
    WHERE l.CompanyId = @company_id
      AND LOWER(je.[Status]) = 'posted'
      AND je.EntryDate >= @from_date AND je.EntryDate < DATEADD(DAY, 1, @to_date)
      AND a.Active = 1
    GROUP BY a.Code, a.Name
    ORDER BY a.Code;
END
GO

CREATE OR ALTER PROCEDURE [fin].[sp_report_income_statement_puc]
(
    @company_id INT,
    @from_date DATETIME2,
    @to_date DATETIME2
)
AS
BEGIN
    SET NOCOUNT ON;

    -- PyG por PUC: ingresos (4), costos (6), gastos (5,7)
    SELECT
        LEFT(a.Code, 1) AS ClassCode,
        CASE LEFT(a.Code, 1)
            WHEN '4' THEN 'INGRESOS'
            WHEN '5' THEN 'GASTOS'
            WHEN '6' THEN 'COSTOS'
            WHEN '7' THEN 'COSTOS/GP'
            ELSE 'OTROS'
        END AS ClassName,
        SUM(l.Debit) AS Debit,
        SUM(l.Credit) AS Credit,
        SUM(l.Credit - l.Debit) AS Net
    FROM [fin].[JournalEntryLine] l WITH (NOLOCK)
    INNER JOIN [fin].[JournalEntry] je WITH (NOLOCK) ON je.Id = l.JournalEntryId AND je.CompanyId = l.CompanyId
    INNER JOIN [fin].[Account] a WITH (NOLOCK) ON a.Id = l.AccountId AND a.CompanyId = l.CompanyId
    WHERE l.CompanyId = @company_id
      AND LOWER(je.[Status]) = 'posted'
      AND je.EntryDate >= @from_date AND je.EntryDate < DATEADD(DAY, 1, @to_date)
      AND LEFT(a.Code, 1) IN ('4','5','6','7')
      AND a.Active = 1
    GROUP BY LEFT(a.Code, 1)
    ORDER BY LEFT(a.Code, 1);
END
GO

CREATE OR ALTER PROCEDURE [fin].[sp_report_balance_sheet_puc]
(
    @company_id INT,
    @to_date DATETIME2
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Balance general por PUC: Activo(1), Pasivo(2), Patrimonio(3)
    SELECT
        LEFT(a.Code, 1) AS ClassCode,
        CASE LEFT(a.Code, 1)
            WHEN '1' THEN 'ACTIVO'
            WHEN '2' THEN 'PASIVO'
            WHEN '3' THEN 'PATRIMONIO'
            ELSE 'OTROS'
        END AS ClassName,
        SUM(l.Debit) AS Debit,
        SUM(l.Credit) AS Credit,
        SUM(l.Debit - l.Credit) AS Balance
    FROM [fin].[JournalEntryLine] l WITH (NOLOCK)
    INNER JOIN [fin].[JournalEntry] je WITH (NOLOCK) ON je.Id = l.JournalEntryId AND je.CompanyId = l.CompanyId
    INNER JOIN [fin].[Account] a WITH (NOLOCK) ON a.Id = l.AccountId AND a.CompanyId = l.CompanyId
    WHERE l.CompanyId = @company_id
      AND LOWER(je.[Status]) = 'posted'
      AND je.EntryDate < DATEADD(DAY, 1, @to_date)
      AND LEFT(a.Code, 1) IN ('1','2','3')
      AND a.Active = 1
    GROUP BY LEFT(a.Code, 1)
    ORDER BY LEFT(a.Code, 1);
END
GO

/* =========================================================
   BUSINESS (Ubicaciones: bodega/zona/estante/vitrina/bin)
   ========================================================= */

CREATE OR ALTER PROCEDURE [business].[sp_upsert_storage_location]
(
    @company_id INT,
    @id BIGINT = NULL,
    @facility_id INT = NULL,
    @code NVARCHAR(50),
    @name NVARCHAR(200),
    @type NVARCHAR(30),
    @parent_location_id BIGINT = NULL,
    @notes NVARCHAR(500) = NULL,
    @active BIT = 1,
    @performed_by_user_id INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @id IS NULL OR @id = 0
    BEGIN
        INSERT INTO [business].[StorageLocation]
            (CompanyId, FacilityId, Code, Name, [Type], ParentLocationId, Notes, Active, CreatedAt, CreatedByUserId, UpdatedAt, UpdatedByUserId)
        VALUES
            (@company_id, @facility_id, @code, @name, @type, @parent_location_id, @notes, @active, SYSUTCDATETIME(), @performed_by_user_id, SYSUTCDATETIME(), @performed_by_user_id);

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
        RETURN;
    END

    UPDATE [business].[StorageLocation]
    SET FacilityId = @facility_id,
        Code = @code,
        Name = @name,
        [Type] = @type,
        ParentLocationId = @parent_location_id,
        Notes = @notes,
        Active = @active,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @performed_by_user_id
    WHERE Id = @id AND CompanyId = @company_id;

    SELECT @id AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [business].[sp_list_storage_locations]
(
    @company_id INT,
    @facility_id INT = NULL,
    @only_active BIT = 1,
    @page INT = 1,
    @page_size INT = 50
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@page - 1) * @page_size;

    SELECT
        l.Id,
        l.CompanyId,
        l.FacilityId,
        l.Code,
        l.Name,
        l.[Type],
        l.ParentLocationId,
        l.Notes,
        l.Active,
        l.CreatedAt,
        l.UpdatedAt
    FROM [business].[StorageLocation] l WITH (NOLOCK)
    WHERE l.CompanyId = @company_id
      AND (@facility_id IS NULL OR l.FacilityId = @facility_id OR l.FacilityId IS NULL)
      AND (@only_active = 0 OR l.Active = 1)
    ORDER BY l.Code
    OFFSET @Offset ROWS FETCH NEXT @page_size ROWS ONLY;

    SELECT COUNT(1) AS Total
    FROM [business].[StorageLocation] l WITH (NOLOCK)
    WHERE l.CompanyId = @company_id
      AND (@facility_id IS NULL OR l.FacilityId = @facility_id OR l.FacilityId IS NULL)
      AND (@only_active = 0 OR l.Active = 1);
END
GO

/* =========================================================
   COMMERCE (Inventario por ubicación)
   ========================================================= */

CREATE OR ALTER PROCEDURE [com].[sp_report_inventory_by_location]
(
    @company_id INT,
    @as_of DATETIME2,
    @location_id BIGINT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH Mov AS (
        SELECT
            m.ProductId,
            COALESCE(m.ToLocationId, m.FromLocationId) AS LocationId,
            CASE
                WHEN m.Type = 'IN' THEN m.Quantity
                WHEN m.Type = 'OUT' THEN -m.Quantity
                WHEN m.Type = 'ADJUST' THEN m.Quantity
                ELSE 0
            END AS SignedQty
        FROM [com].[InventoryMovement] m WITH (NOLOCK)
        WHERE m.CompanyId = @company_id
          AND m.MoveDate < DATEADD(DAY, 1, @as_of)
          AND (m.ToLocationId IS NOT NULL OR m.FromLocationId IS NOT NULL)
          AND (@location_id IS NULL OR m.ToLocationId = @location_id OR m.FromLocationId = @location_id)
    )
    SELECT
        p.Id AS ProductId,
        p.Sku,
        p.Name AS ProductName,
        m.LocationId,
        l.Code AS LocationCode,
        l.Name AS LocationName,
        SUM(m.SignedQty) AS QuantityOnHand
    FROM Mov m
    INNER JOIN [com].[Product] p WITH (NOLOCK) ON p.Id = m.ProductId AND p.CompanyId = @company_id
    LEFT JOIN [business].[StorageLocation] l WITH (NOLOCK) ON l.Id = m.LocationId AND l.CompanyId = @company_id
    GROUP BY p.Id, p.Sku, p.Name, m.LocationId, l.Code, l.Name
    HAVING ABS(SUM(m.SignedQty)) > 0.00001
    ORDER BY l.Code, p.Sku;
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
        c.Name AS CountryName
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
        de.Name AS DepartmentName,
        de.CountryId,
        co.Name AS CountryName
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
    SELECT Id, Name, Abbreviation, Active
    FROM [common].[DocumentType]
    WHERE Active = 1;
END
GO

CREATE OR ALTER PROCEDURE [common].[sp_get_document_type_by_id]
    @Id INT
AS
BEGIN
    SELECT Id, Name, Abbreviation, Active
    FROM [common].[DocumentType]
    WHERE Id = @Id AND Active = 1;
END
GO

/* =========================================================
   AUDIT
   ========================================================= */

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
    INSERT INTO [audit].[UserSession]
    (UserId, LoginField, CompanyId, Platform, DeviceFingerprint, IpAddress, UserAgent, LoginAt, IsSuccess, FailureReason)
    VALUES
    (@UserId, @LoginField, @CompanyId, @Platform, @DeviceFingerprint, @IpAddress, @UserAgent, @LoginAt, @IsSuccess, @FailureReason);

    SELECT SCOPE_IDENTITY();
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

/* =========================================================
   AUTH
   ========================================================= */

CREATE OR ALTER PROCEDURE [auth].[sp_login_user]
(
    @username VARCHAR(150),
    @company_id INT,
    @application_code NVARCHAR(200) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1
        FROM [config].[ApplicationCompanies] WITH (NOLOCK)
        WHERE CompanyId = @company_id
          AND IsEnabled = 1
          AND Active = 1
          AND (
              @application_code IS NULL
              OR LTRIM(RTRIM(@application_code)) = ''
              OR LTRIM(RTRIM(ApplicationCode)) = LTRIM(RTRIM(@application_code))
          )
    )
    BEGIN
        RAISERROR('La compañía no tiene una configuración de aplicación activa para este cliente (applicationCode).', 16, 1);
        RETURN;
    END

    ;WITH CompanyConfig AS (
        SELECT LoginField, Priority
        FROM [config].[ApplicationCompanies] WITH (NOLOCK)
        WHERE CompanyId = @company_id
          AND IsEnabled = 1
          AND Active = 1
          AND (
              @application_code IS NULL
              OR LTRIM(RTRIM(@application_code)) = ''
              OR LTRIM(RTRIM(ApplicationCode)) = LTRIM(RTRIM(@application_code))
          )
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
        r.Name AS RoleName,
        r.IsSystemRole,
        r.IsSuperUser,
        c.Name AS CompanyName,
        c.Id AS CompanyId
    FROM [auth].[UsersInfo] ui WITH (NOLOCK)
    INNER JOIN [business].[Companies] c WITH (NOLOCK) ON ui.CompanyId = c.Id
    LEFT JOIN [config].[Role] r WITH (NOLOCK) ON r.Id = ui.RoleId
    CROSS JOIN CompanyConfig cc
    WHERE ui.CompanyId = @company_id
      AND ui.Active = 1
      AND ui.CanLogin = 1
      AND c.Active = 1
      AND (
        (cc.LoginField = 'email' AND ui.Email = @username) OR
        (cc.LoginField = 'phone' AND ui.Phone = @username) OR
        (cc.LoginField IN ('document', 'documentNumber') AND ui.DocumentNumber = @username)
      )
    ORDER BY cc.Priority;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Usuario no encontrado o método de login no válido para esta compañía.', 16, 1);
    END
END
GO

CREATE OR ALTER PROCEDURE [auth].[sp_login_system_user]
(
    @username VARCHAR(150)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Login sin compañía: solo para usuarios con rol de sistema (y típicamente superusuario).
    SELECT TOP 1
        ui.Id AS UserId,
        @username AS UserName,
        ui.DocumentNumber,
        ui.FirstName,
        ui.LastName,
        ui.Email,
        ui.Phone,
        ui.Password,
        ui.RoleId,
        r.Name AS RoleName,
        r.IsSystemRole,
        r.IsSuperUser,
        CAST(0 AS INT) AS CompanyId,
        CAST(NULL AS NVARCHAR(200)) AS CompanyName
    FROM [auth].[UsersInfo] ui WITH (NOLOCK)
    INNER JOIN [config].[Role] r WITH (NOLOCK) ON r.Id = ui.RoleId
    WHERE ui.Active = 1
      AND ui.CanLogin = 1
      AND r.IsSystemRole = 1
      AND (ui.CompanyId IS NULL OR ui.CompanyId = 0)
      AND (
        (ui.Email = @username) OR
        (ui.Phone = @username) OR
        (ui.DocumentNumber = @username)
      )
    ORDER BY r.IsSuperUser DESC, ui.Id DESC;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Usuario de sistema no encontrado o no autorizado.', 16, 1);
    END
END
GO

/* =========================================================
   AUTH (Acceso público solo lectura)
   ========================================================= */

CREATE OR ALTER PROCEDURE [auth].[sp_create_public_access_token]
(
    @company_id INT,
    @user_id INT,
    @token_hash NVARCHAR(64),
    @scope NVARCHAR(30),
    @expires_at DATETIME2
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Revocar tokens previos del mismo scope (simple y seguro)
    UPDATE [auth].[PublicAccessToken]
    SET Active = 0
    WHERE CompanyId = @company_id AND UserId = @user_id AND Scope = @scope AND Active = 1;

    INSERT INTO [auth].[PublicAccessToken]
        (CompanyId, UserId, TokenHash, Scope, ExpiresAt, Active, CreatedAt)
    VALUES
        (@company_id, @user_id, @token_hash, @scope, @expires_at, 1, SYSUTCDATETIME());

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [auth].[sp_get_public_hr_account_summary]
(
    @token_hash NVARCHAR(64)
)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @now DATETIME2 = SYSUTCDATETIME();

    DECLARE @company_id INT;
    DECLARE @user_id INT;

    SELECT TOP 1
        @company_id = t.CompanyId,
        @user_id = t.UserId
    FROM [auth].[PublicAccessToken] t WITH (NOLOCK)
    WHERE t.TokenHash = @token_hash
      AND t.Scope = 'HR_ACCOUNT'
      AND t.Active = 1
      AND t.ExpiresAt > @now;

    IF @company_id IS NULL OR @user_id IS NULL
    BEGIN
        RAISERROR('Invalid or expired token.', 16, 1);
        RETURN;
    END

    -- Resumen simple: ingresos (WorkLogs) - deducciones - préstamos/avances (pendiente vs pagado lo refinamos luego)
    SELECT
        @company_id AS CompanyId,
        @user_id AS UserId,
        ISNULL(SUM(w.Amount), 0) AS TotalEarned,
        ISNULL((SELECT SUM(d.Amount) FROM [hr].[Deduction] d WITH (NOLOCK) WHERE d.CompanyId = @company_id AND d.UserId = @user_id), 0) AS TotalDeductions,
        ISNULL((SELECT SUM(l.Amount) FROM [hr].[LoanAdvance] l WITH (NOLOCK) WHERE l.CompanyId = @company_id AND l.UserId = @user_id), 0) AS TotalLoans,
        ISNULL(SUM(w.Amount), 0)
          - ISNULL((SELECT SUM(d.Amount) FROM [hr].[Deduction] d WITH (NOLOCK) WHERE d.CompanyId = @company_id AND d.UserId = @user_id), 0)
          - ISNULL((SELECT SUM(l.Amount) FROM [hr].[LoanAdvance] l WITH (NOLOCK) WHERE l.CompanyId = @company_id AND l.UserId = @user_id), 0) AS Net
    FROM [hr].[WorkLog] w WITH (NOLOCK)
    WHERE w.CompanyId = @company_id AND w.UserId = @user_id;
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

CREATE OR ALTER PROCEDURE [auth].[sp_update_user]
(
    @CompanyId INT,
    @UserId INT,
    @DocumentTypeId INT,
    @DocumentNumber VARCHAR(50),
    @FirstName VARCHAR(150),
    @LastName VARCHAR(150),
    @Email VARCHAR(150),
    @Phone VARCHAR(20),
    @PasswordHash NVARCHAR(255),
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
    WHERE Id = @UserId AND CompanyId = @CompanyId;
END
GO

CREATE OR ALTER PROCEDURE [auth].[sp_delete_user]
(
    @CompanyId INT,
    @UserId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [auth].[UsersInfo]
    SET Active = 0, Updated = SYSUTCDATETIME()
    WHERE Id = @UserId AND CompanyId = @CompanyId;
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

    SELECT COUNT(1) AS Total
    FROM [auth].[UsersInfo]
    WHERE CompanyId = @CompanyId;
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

CREATE OR ALTER PROCEDURE [auth].[sp_get_rol_user]
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
    INNER JOIN [config].[Role] r WITH (NOLOCK) ON ui.RoleId = r.Id
    WHERE ui.Id = @UserId;
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

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
END
GO

CREATE OR ALTER PROCEDURE [auth].[sp_get_active_refresh_token]
    @Token NVARCHAR(500),
    @CurrentTime DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        rt.Token AS RefreshToken,
        rt.JwtId,
        rt.AccessTokenExpiresAt,
        rt.UserSessionId,
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
        CASE
            WHEN us.LoginField = 'email' THEN u.Email
            WHEN us.LoginField = 'phone' THEN u.Phone
            WHEN us.LoginField = 'document' THEN u.DocumentNumber
        END AS UserName
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
    @UserSessionId BIGINT,
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
      AND Active = 1;
END
GO

CREATE OR ALTER PROCEDURE [auth].[sp_revoke_all_tokens_by_user_id]
    @UserId INT,
    @RevokedAt DATETIME,
    @CurrentTime DATETIME,
    @ReplacedByToken NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE rt
    SET rt.RevokedAt = @RevokedAt,
        rt.ReplacedByToken = @ReplacedByToken,
        rt.Active = 0
    FROM [audit].[RefreshToken] rt WITH (NOLOCK)
    INNER JOIN [audit].[UserSession] us WITH (NOLOCK) ON rt.UserSessionId = us.Id
    WHERE us.UserId = @UserId
      AND rt.RevokedAt IS NULL
      AND rt.ExpiresAt > @CurrentTime
      AND rt.Active = 1;
END
GO

CREATE OR ALTER PROCEDURE [auth].[sp_revoke_all_tokens_by_company]
    @CompanyId INT,
    @RevokedAt DATETIME,
    @CurrentTime DATETIME,
    @ReplacedByToken NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE rt
    SET rt.RevokedAt = @RevokedAt,
        rt.ReplacedByToken = @ReplacedByToken,
        rt.Active = 0
    FROM [audit].[RefreshToken] rt WITH (NOLOCK)
    INNER JOIN [audit].[UserSession] us WITH (NOLOCK) ON rt.UserSessionId = us.Id
    WHERE us.CompanyId = @CompanyId
      AND rt.RevokedAt IS NULL
      AND rt.ExpiresAt > @CurrentTime
      AND rt.Active = 1;
END
GO

/* =========================================================
   HR
   ========================================================= */

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
      AND (ep.IsEmployee = 1 OR ep.UserId IS NULL)
    ORDER BY u.FirstName, u.LastName;
END
GO

/* =========================================================
   HR: Disponibilidad opcional (horarios / excepciones / capacidad)
   ========================================================= */

CREATE OR ALTER PROCEDURE [hr].[sp_get_employee_availability]
(
    @company_id INT,
    @user_id INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        p.Id,
        p.CompanyId,
        p.UserId,
        p.TimeZone,
        p.MaxServicesPerDay,
        p.Active,
        p.Notes,
        p.CreatedAt,
        p.UpdatedAt
    FROM [hr].[EmployeeAvailabilityProfile] p WITH (NOLOCK)
    WHERE p.CompanyId=@company_id AND p.UserId=@user_id;

    SELECT
        s.Id,
        s.CompanyId,
        s.UserId,
        s.DayOfWeek,
        s.StartTime,
        s.EndTime,
        s.MaxServicesInSlot,
        s.Active
    FROM [hr].[EmployeeAvailabilitySlot] s WITH (NOLOCK)
    WHERE s.CompanyId=@company_id AND s.UserId=@user_id
    ORDER BY s.DayOfWeek, s.StartTime;

    SELECT
        e.Id,
        e.CompanyId,
        e.UserId,
        e.DateFrom,
        e.DateTo,
        e.IsAvailable,
        e.Reason,
        e.CreatedAt
    FROM [hr].[EmployeeAvailabilityException] e WITH (NOLOCK)
    WHERE e.CompanyId=@company_id AND e.UserId=@user_id
    ORDER BY e.DateFrom DESC, e.Id DESC;
END
GO

CREATE OR ALTER PROCEDURE [hr].[sp_upsert_employee_availability]
(
    @company_id INT,
    @user_id INT,
    @time_zone NVARCHAR(80) = NULL,
    @max_services_per_day INT = NULL,
    @active BIT = 1,
    @notes NVARCHAR(500) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [hr].[EmployeeAvailabilityProfile] WHERE CompanyId=@company_id AND UserId=@user_id)
    BEGIN
        UPDATE [hr].[EmployeeAvailabilityProfile]
        SET TimeZone=@time_zone,
            MaxServicesPerDay=@max_services_per_day,
            Active=@active,
            Notes=@notes,
            UpdatedAt=SYSUTCDATETIME()
        WHERE CompanyId=@company_id AND UserId=@user_id;
    END
    ELSE
    BEGIN
        INSERT INTO [hr].[EmployeeAvailabilityProfile]
            (CompanyId, UserId, TimeZone, MaxServicesPerDay, Active, Notes, CreatedAt, UpdatedAt)
        VALUES
            (@company_id, @user_id, @time_zone, @max_services_per_day, @active, @notes, SYSUTCDATETIME(), SYSUTCDATETIME());
    END

    SELECT CAST(1 AS BIT) AS Success;
END
GO

CREATE OR ALTER PROCEDURE [hr].[sp_set_employee_availability_slots]
(
    @company_id INT,
    @user_id INT,
    @slots_json NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @slots_json IS NULL OR ISJSON(@slots_json) <> 1
    BEGIN
        RAISERROR('slots_json must be valid JSON array.', 16, 1);
        RETURN;
    END

    DELETE FROM [hr].[EmployeeAvailabilitySlot]
    WHERE CompanyId=@company_id AND UserId=@user_id;

    INSERT INTO [hr].[EmployeeAvailabilitySlot]
        (CompanyId, UserId, DayOfWeek, StartTime, EndTime, MaxServicesInSlot, Active)
    SELECT
        @company_id,
        @user_id,
        j.DayOfWeek,
        j.StartTime,
        j.EndTime,
        j.MaxServicesInSlot,
        COALESCE(j.Active, 1)
    FROM OPENJSON(@slots_json)
    WITH
    (
        DayOfWeek INT '$.dayOfWeek',
        StartTime TIME '$.startTime',
        EndTime TIME '$.endTime',
        MaxServicesInSlot INT '$.maxServicesInSlot',
        Active BIT '$.active'
    ) j
    WHERE j.DayOfWeek BETWEEN 0 AND 6
      AND j.StartTime IS NOT NULL
      AND j.EndTime IS NOT NULL
      AND j.EndTime > j.StartTime;

    SELECT CAST(1 AS BIT) AS Success;
END
GO

CREATE OR ALTER PROCEDURE [hr].[sp_set_employee_availability_exceptions]
(
    @company_id INT,
    @user_id INT,
    @exceptions_json NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @exceptions_json IS NULL OR ISJSON(@exceptions_json) <> 1
    BEGIN
        RAISERROR('exceptions_json must be valid JSON array.', 16, 1);
        RETURN;
    END

    DELETE FROM [hr].[EmployeeAvailabilityException]
    WHERE CompanyId=@company_id AND UserId=@user_id;

    INSERT INTO [hr].[EmployeeAvailabilityException]
        (CompanyId, UserId, DateFrom, DateTo, IsAvailable, Reason, CreatedAt)
    SELECT
        @company_id,
        @user_id,
        j.DateFrom,
        j.DateTo,
        COALESCE(j.IsAvailable, 0),
        j.Reason,
        SYSUTCDATETIME()
    FROM OPENJSON(@exceptions_json)
    WITH
    (
        DateFrom DATE '$.dateFrom',
        DateTo DATE '$.dateTo',
        IsAvailable BIT '$.isAvailable',
        Reason NVARCHAR(300) '$.reason'
    ) j
    WHERE j.DateFrom IS NOT NULL
      AND j.DateTo IS NOT NULL
      AND j.DateTo >= j.DateFrom;

    SELECT CAST(1 AS BIT) AS Success;
END
GO

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

/* =========================================================
   FARM
   ========================================================= */

CREATE OR ALTER PROCEDURE [farm].[sp_upsert_zone]
    @company_id INT,
    @code NVARCHAR(50),
    @name NVARCHAR(150),
    @active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [farm].[FarmZone] WHERE CompanyId=@company_id AND Code=@code)
        UPDATE [farm].[FarmZone]
        SET Name=@name, Active=@active, UpdatedAt=SYSUTCDATETIME()
        WHERE CompanyId=@company_id AND Code=@code;
    ELSE
        INSERT INTO [farm].[FarmZone] (CompanyId, Code, Name, Active) VALUES (@company_id, @code, @name, @active);

    SELECT CAST(1 AS BIT) AS Success;
END
GO

CREATE OR ALTER PROCEDURE [farm].[sp_list_zones]
    @company_id INT,
    @only_active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, CompanyId, Code, Name, Active
    FROM [farm].[FarmZone]
    WHERE CompanyId=@company_id AND (@only_active=0 OR Active=1)
    ORDER BY Name;
END
GO

CREATE OR ALTER PROCEDURE [farm].[sp_create_harvest_lot]
    @company_id INT,
    @zone_id INT = NULL,
    @harvest_date DATE,
    @product_form NVARCHAR(50),
    @quantity_kg DECIMAL(18,3),
    @notes NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [farm].[HarvestLot] (CompanyId, ZoneId, HarvestDate, ProductForm, QuantityKg, Notes)
    VALUES (@company_id, @zone_id, @harvest_date, @product_form, @quantity_kg, @notes);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [farm].[sp_list_harvest_lots]
    @company_id INT,
    @from_date DATE = NULL,
    @to_date DATE = NULL,
    @zone_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT hl.Id, hl.CompanyId, hl.ZoneId, z.Name AS ZoneName, hl.HarvestDate, hl.ProductForm, hl.QuantityKg, hl.Notes
    FROM [farm].[HarvestLot] hl
    LEFT JOIN [farm].[FarmZone] z ON z.Id = hl.ZoneId
    WHERE hl.CompanyId=@company_id
      AND (@zone_id IS NULL OR hl.ZoneId=@zone_id)
      AND (@from_date IS NULL OR hl.HarvestDate>=@from_date)
      AND (@to_date IS NULL OR hl.HarvestDate<=@to_date)
    ORDER BY hl.HarvestDate DESC, hl.Id DESC;
END
GO

CREATE OR ALTER PROCEDURE [farm].[sp_create_process_step]
    @company_id INT,
    @harvest_lot_id BIGINT,
    @step_date DATE,
    @step_type NVARCHAR(50),
    @input_kg DECIMAL(18,3) = NULL,
    @output_kg DECIMAL(18,3) = NULL,
    @notes NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [farm].[CoffeeProcessStep] (CompanyId, HarvestLotId, StepDate, StepType, InputKg, OutputKg, Notes)
    VALUES (@company_id, @harvest_lot_id, @step_date, @step_type, @input_kg, @output_kg, @notes);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [farm].[sp_list_process_steps]
    @company_id INT,
    @harvest_lot_id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, CompanyId, HarvestLotId, StepDate, StepType, InputKg, OutputKg, Notes
    FROM [farm].[CoffeeProcessStep]
    WHERE CompanyId=@company_id AND HarvestLotId=@harvest_lot_id
    ORDER BY StepDate ASC, Id ASC;
END
GO

/* =========================================================
   SERVICES
   ========================================================= */

CREATE OR ALTER PROCEDURE [svc].[sp_upsert_service]
    @company_id INT,
    @code NVARCHAR(50),
    @name NVARCHAR(150),
    @base_price DECIMAL(18,2),
    @active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [svc].[ServiceCatalog] WHERE CompanyId=@company_id AND Code=@code)
        UPDATE [svc].[ServiceCatalog]
        SET Name=@name, BasePrice=@base_price, Active=@active, UpdatedAt=SYSUTCDATETIME()
        WHERE CompanyId=@company_id AND Code=@code;
    ELSE
        INSERT INTO [svc].[ServiceCatalog] (CompanyId, Code, Name, BasePrice, Active)
        VALUES (@company_id, @code, @name, @base_price, @active);

    SELECT CAST(1 AS BIT) AS Success;
END
GO

CREATE OR ALTER PROCEDURE [svc].[sp_list_services]
    @company_id INT,
    @only_active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, CompanyId, Code, Name, BasePrice, Active
    FROM [svc].[ServiceCatalog]
    WHERE CompanyId=@company_id AND (@only_active=0 OR Active=1)
    ORDER BY Name;
END
GO

CREATE OR ALTER PROCEDURE [svc].[sp_create_service_order]
    @company_id INT,
    @vehicle_type NVARCHAR(50) = NULL,
    @plate NVARCHAR(20) = NULL,
    @customer_name NVARCHAR(150) = NULL,
    @fulfillment_method_code NVARCHAR(30) = NULL,
    @fulfillment_details NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [svc].[ServiceOrder] (CompanyId, VehicleType, Plate, CustomerName, FulfillmentMethodCode, FulfillmentDetails)
    VALUES (@company_id, @vehicle_type, @plate, @customer_name, @fulfillment_method_code, @fulfillment_details);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [svc].[sp_add_service_order_line]
    @company_id INT,
    @order_id BIGINT,
    @service_id INT,
    @quantity DECIMAL(18,3),
    @unit_price DECIMAL(18,2),
    @employee_user_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [svc].[ServiceOrderLine] (CompanyId, OrderId, ServiceId, Quantity, UnitPrice, EmployeeUserId)
    VALUES (@company_id, @order_id, @service_id, @quantity, @unit_price, @employee_user_id);

    UPDATE [svc].[ServiceOrder]
    SET Total = (
        SELECT COALESCE(SUM(LineTotal),0)
        FROM [svc].[ServiceOrderLine]
        WHERE CompanyId=@company_id AND OrderId=@order_id
    )
    WHERE CompanyId=@company_id AND Id=@order_id;

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [svc].[sp_get_service_order]
    @company_id INT,
    @order_id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 Id, CompanyId, OrderDate, VehicleType, Plate, CustomerName, FulfillmentMethodCode, FulfillmentDetails, Status, Total, CreatedAt
    FROM [svc].[ServiceOrder]
    WHERE CompanyId=@company_id AND Id=@order_id;

    SELECT l.Id, l.OrderId, l.ServiceId, s.Name AS ServiceName, l.Quantity, l.UnitPrice, l.LineTotal, l.EmployeeUserId
    FROM [svc].[ServiceOrderLine] l
    INNER JOIN [svc].[ServiceCatalog] s ON s.Id=l.ServiceId
    WHERE l.CompanyId=@company_id AND l.OrderId=@order_id
    ORDER BY l.Id;
END
GO

CREATE OR ALTER PROCEDURE [svc].[sp_list_service_orders]
(
    @company_id INT,
    @status NVARCHAR(20) = NULL,
    @page INT = 1,
    @page_size INT = 50
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@page - 1) * @page_size;

    SELECT
        o.Id,
        o.CompanyId,
        o.OrderDate,
        o.VehicleType,
        o.Plate,
        o.CustomerName,
        o.FulfillmentMethodCode,
        o.FulfillmentDetails,
        o.[Status],
        o.Total,
        o.CreatedAt
    FROM [svc].[ServiceOrder] o
    WHERE o.CompanyId = @company_id
      AND (@status IS NULL OR LOWER(LTRIM(RTRIM(o.[Status]))) = LOWER(LTRIM(RTRIM(@status))))
    ORDER BY o.OrderDate DESC, o.Id DESC
    OFFSET @Offset ROWS FETCH NEXT @page_size ROWS ONLY;

    SELECT COUNT(1) AS Total
    FROM [svc].[ServiceOrder] o
    WHERE o.CompanyId = @company_id
      AND (@status IS NULL OR LOWER(LTRIM(RTRIM(o.[Status]))) = LOWER(LTRIM(RTRIM(@status))));
END
GO

/* =========================================================
   COMMERCE / POS
   ========================================================= */

CREATE OR ALTER PROCEDURE [com].[sp_upsert_product]
    @company_id INT,
    @sku NVARCHAR(50),
    @name NVARCHAR(200),
    @unit NVARCHAR(20) = NULL,
    @price DECIMAL(18,2),
    @active BIT = 1,
    @performed_by_user_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [com].[Product] WHERE CompanyId=@company_id AND Sku=@sku)
        UPDATE [com].[Product]
        SET Name=@name, Unit=@unit, Price=@price, Active=@active, UpdatedAt=SYSUTCDATETIME(), UpdatedByUserId=@performed_by_user_id
        WHERE CompanyId=@company_id AND Sku=@sku;
    ELSE
        INSERT INTO [com].[Product] (CompanyId, Sku, Name, Unit, Price, Active, CreatedAt, CreatedByUserId, UpdatedAt, UpdatedByUserId)
        VALUES (@company_id, @sku, @name, @unit, @price, @active, SYSUTCDATETIME(), @performed_by_user_id, SYSUTCDATETIME(), @performed_by_user_id);

    SELECT CAST(1 AS BIT) AS Success;
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_list_products]
    @company_id INT,
    @only_active BIT = 1,
    @page INT = 1,
    @page_size INT = 50,
    @q NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@page - 1) * @page_size;

    SELECT Id, CompanyId, Sku, Name, Unit, Price, Active
    FROM [com].[Product]
    WHERE CompanyId=@company_id AND (@only_active=0 OR Active=1)
      AND (@q IS NULL OR Sku LIKE '%' + @q + '%' OR Name LIKE '%' + @q + '%')
    ORDER BY Name
    OFFSET @Offset ROWS FETCH NEXT @page_size ROWS ONLY;

    SELECT COUNT(1) AS Total
    FROM [com].[Product]
    WHERE CompanyId=@company_id AND (@only_active=0 OR Active=1)
      AND (@q IS NULL OR Sku LIKE '%' + @q + '%' OR Name LIKE '%' + @q + '%');
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_create_cash_session]
    @company_id INT,
    @opened_by_user_id INT = NULL,
    @opening_amount DECIMAL(18,2) = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Solo una caja abierta por usuario (por compañía)
    IF @opened_by_user_id IS NOT NULL AND EXISTS (
        SELECT 1 FROM [com].[CashSession] cs WITH (NOLOCK)
        WHERE cs.CompanyId=@company_id AND cs.OpenedByUserId=@opened_by_user_id AND LOWER(cs.[Status])='open'
    )
    BEGIN
        RAISERROR('User already has an open cash session.', 16, 1);
        RETURN;
    END

    INSERT INTO [com].[CashSession] (CompanyId, OpenedAt, OpenedByUserId, [Status], OpeningAmount)
    VALUES (@company_id, SYSUTCDATETIME(), @opened_by_user_id, 'open', @opening_amount);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_add_cash_movement]
(
    @company_id INT,
    @cash_session_id BIGINT,
    @direction NVARCHAR(10),
    @reason_code NVARCHAR(30) = NULL,
    @amount DECIMAL(18,2),
    @reference_type NVARCHAR(100) = NULL,
    @reference_id NVARCHAR(100) = NULL,
    @notes NVARCHAR(500) = NULL,
    @performed_by_user_id INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @amount <= 0
    BEGIN
        RAISERROR('Amount must be > 0.', 16, 1);
        RETURN;
    END

    IF LOWER(@direction) NOT IN ('in','out')
    BEGIN
        RAISERROR('Direction must be IN or OUT.', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (
        SELECT 1 FROM [com].[CashSession] cs WITH (NOLOCK)
        WHERE cs.CompanyId=@company_id AND cs.Id=@cash_session_id AND LOWER(cs.[Status])='open'
    )
    BEGIN
        RAISERROR('CashSession not found or not open.', 16, 1);
        RETURN;
    END

    INSERT INTO [com].[CashMovement]
        (CompanyId, CashSessionId, Direction, ReasonCode, Amount, ReferenceType, ReferenceId, Notes, PerformedByUserId, CreatedAt)
    VALUES
        (@company_id, @cash_session_id, UPPER(@direction), @reason_code, @amount, @reference_type, @reference_id, @notes, @performed_by_user_id, SYSUTCDATETIME());

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_get_cash_session]
(
    @company_id INT,
    @cash_session_id BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        cs.Id,
        cs.CompanyId,
        cs.OpenedAt,
        cs.ClosedAt,
        cs.OpenedByUserId,
        cs.ClosedByUserId,
        cs.[Status],
        cs.OpeningAmount,
        cs.ExpectedClosingAmount,
        cs.CountedClosingAmount,
        cs.DifferenceAmount
    FROM [com].[CashSession] cs WITH (NOLOCK)
    WHERE cs.CompanyId=@company_id AND cs.Id=@cash_session_id;

    SELECT
        m.Id,
        m.CashSessionId,
        m.Direction,
        m.ReasonCode,
        m.Amount,
        m.ReferenceType,
        m.ReferenceId,
        m.Notes,
        m.PerformedByUserId,
        m.CreatedAt
    FROM [com].[CashMovement] m WITH (NOLOCK)
    WHERE m.CompanyId=@company_id AND m.CashSessionId=@cash_session_id
    ORDER BY m.Id;
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_close_cash_session]
(
    @company_id INT,
    @cash_session_id BIGINT,
    @counted_closing_amount DECIMAL(18,2),
    @closed_by_user_id INT = NULL,
    @cash_payment_method_code NVARCHAR(30) = 'CASH',
    @create_over_short_entry BIT = 1,
    @cash_account_code NVARCHAR(20) = '110505',
    @over_short_expense_account_code NVARCHAR(20) = '519595',
    @over_short_income_account_code NVARCHAR(20) = '429595'
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1 FROM [com].[CashSession] cs WITH (NOLOCK)
        WHERE cs.CompanyId=@company_id AND cs.Id=@cash_session_id AND LOWER(cs.[Status])='open'
    )
    BEGIN
        RAISERROR('CashSession not found or not open.', 16, 1);
        RETURN;
    END

    DECLARE @opening DECIMAL(18,2) = 0;
    SELECT @opening = cs.OpeningAmount FROM [com].[CashSession] cs WITH (NOLOCK)
    WHERE cs.CompanyId=@company_id AND cs.Id=@cash_session_id;

    DECLARE @cashSales DECIMAL(18,2) = 0;
    SELECT @cashSales = COALESCE(SUM(p.Amount),0)
    FROM [com].[PosPayment] p WITH (NOLOCK)
    INNER JOIN [com].[PosTicket] tk WITH (NOLOCK) ON tk.Id=p.TicketId AND tk.CompanyId=p.CompanyId
    WHERE p.CompanyId=@company_id
      AND tk.CashSessionId=@cash_session_id
      AND LOWER(p.Method)=LOWER(@cash_payment_method_code);

    DECLARE @movIn DECIMAL(18,2) = 0;
    DECLARE @movOut DECIMAL(18,2) = 0;
    SELECT
        @movIn = COALESCE(SUM(CASE WHEN LOWER(Direction)='in' THEN Amount ELSE 0 END),0),
        @movOut = COALESCE(SUM(CASE WHEN LOWER(Direction)='out' THEN Amount ELSE 0 END),0)
    FROM [com].[CashMovement] m WITH (NOLOCK)
    WHERE m.CompanyId=@company_id AND m.CashSessionId=@cash_session_id;

    DECLARE @expected DECIMAL(18,2) = @opening + @cashSales + @movIn - @movOut;
    DECLARE @diff DECIMAL(18,2) = @counted_closing_amount - @expected;

    UPDATE [com].[CashSession]
    SET ClosedAt = SYSUTCDATETIME(),
        ClosedByUserId = @closed_by_user_id,
        [Status] = 'closed',
        ExpectedClosingAmount = @expected,
        CountedClosingAmount = @counted_closing_amount,
        DifferenceAmount = @diff
    WHERE CompanyId=@company_id AND Id=@cash_session_id;

    -- Asiento opcional por diferencia de arqueo (sobrante/faltante)
    IF @create_over_short_entry = 1 AND ABS(@diff) > 0.009
    BEGIN
        DECLARE @acc_cash BIGINT = NULL;
        DECLARE @acc_exp BIGINT = NULL;
        DECLARE @acc_inc BIGINT = NULL;

        SELECT TOP 1 @acc_cash = a.Id FROM [fin].[Account] a WITH (NOLOCK)
        WHERE a.CompanyId=@company_id AND a.Active=1 AND a.Code=@cash_account_code;

        SELECT TOP 1 @acc_exp = a.Id FROM [fin].[Account] a WITH (NOLOCK)
        WHERE a.CompanyId=@company_id AND a.Active=1 AND a.Code=@over_short_expense_account_code;

        SELECT TOP 1 @acc_inc = a.Id FROM [fin].[Account] a WITH (NOLOCK)
        WHERE a.CompanyId=@company_id AND a.Active=1 AND a.Code=@over_short_income_account_code;

        IF @acc_cash IS NOT NULL AND @acc_exp IS NOT NULL AND @acc_inc IS NOT NULL
        BEGIN
            INSERT INTO [fin].[JournalEntry]
                (CompanyId, EntryDate, EntryType, ReferenceType, ReferenceId, [Description], [Status], CreatedByUserId, CreatedAt, UpdatedAt)
            VALUES
                (@company_id, SYSUTCDATETIME(), 'CASH_SESSION', 'CASH_SESSION', CAST(@cash_session_id AS NVARCHAR(100)),
                 'Arqueo/cierre de caja (diferencia)', 'posted', @closed_by_user_id, SYSUTCDATETIME(), SYSUTCDATETIME());

            DECLARE @je_id BIGINT = CAST(SCOPE_IDENTITY() AS BIGINT);

            IF @diff < 0
            BEGIN
                -- faltante: Dr gasto, Cr caja
                INSERT INTO [fin].[JournalEntryLine] (CompanyId, JournalEntryId, AccountId, Debit, Credit, Notes, CreatedAt)
                VALUES
                    (@company_id, @je_id, @acc_exp, ABS(@diff), 0, 'Faltante de caja', SYSUTCDATETIME()),
                    (@company_id, @je_id, @acc_cash, 0, ABS(@diff), 'Salida por faltante', SYSUTCDATETIME());
            END
            ELSE
            BEGIN
                -- sobrante: Dr caja, Cr ingreso
                INSERT INTO [fin].[JournalEntryLine] (CompanyId, JournalEntryId, AccountId, Debit, Credit, Notes, CreatedAt)
                VALUES
                    (@company_id, @je_id, @acc_cash, @diff, 0, 'Entrada por sobrante', SYSUTCDATETIME()),
                    (@company_id, @je_id, @acc_inc, 0, @diff, 'Sobrante de caja', SYSUTCDATETIME());
            END
        END
    END

    SELECT CAST(1 AS BIT) AS Success, @expected AS Expected, @diff AS Difference;
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_create_pos_ticket]
    @company_id INT,
    @cash_session_id BIGINT = NULL,
    @fulfillment_method_code NVARCHAR(30) = NULL,
    @fulfillment_details NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [com].[PosTicket] (CompanyId, CashSessionId, FulfillmentMethodCode, FulfillmentDetails)
    VALUES (@company_id, @cash_session_id, @fulfillment_method_code, @fulfillment_details);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_get_pos_ticket]
(
    @company_id INT,
    @ticket_id BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        tk.Id,
        tk.CompanyId,
        tk.CashSessionId,
        tk.TicketDate,
        tk.[Status],
        tk.Total,
        tk.FulfillmentMethodCode,
        tk.FulfillmentDetails
    FROM [com].[PosTicket] tk WITH (NOLOCK)
    WHERE tk.CompanyId = @company_id AND tk.Id = @ticket_id;

    SELECT
        l.Id,
        l.TicketId,
        l.ProductId,
        p.Sku AS ProductSku,
        p.Name AS ProductName,
        l.Quantity,
        l.UnitPrice,
        l.LineTotal
    FROM [com].[PosTicketLine] l WITH (NOLOCK)
    INNER JOIN [com].[Product] p WITH (NOLOCK) ON p.Id = l.ProductId AND p.CompanyId = l.CompanyId
    WHERE l.CompanyId = @company_id AND l.TicketId = @ticket_id
    ORDER BY l.Id;

    SELECT
        p.Id,
        p.TicketId,
        p.Method,
        p.Amount,
        p.PaidAt
    FROM [com].[PosPayment] p WITH (NOLOCK)
    WHERE p.CompanyId = @company_id AND p.TicketId = @ticket_id
    ORDER BY p.Id;
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_add_pos_ticket_line]
    @company_id INT,
    @ticket_id BIGINT,
    @product_id INT,
    @quantity DECIMAL(18,3),
    @unit_price DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [com].[PosTicketLine] (CompanyId, TicketId, ProductId, Quantity, UnitPrice)
    VALUES (@company_id, @ticket_id, @product_id, @quantity, @unit_price);

    UPDATE [com].[PosTicket]
    SET Total = (
        SELECT COALESCE(SUM(LineTotal),0)
        FROM [com].[PosTicketLine]
        WHERE CompanyId=@company_id AND TicketId=@ticket_id
    )
    WHERE CompanyId=@company_id AND Id=@ticket_id;

    INSERT INTO [com].[InventoryMovement] (CompanyId, ProductId, Type, Quantity, ReferenceType, ReferenceId)
    VALUES (@company_id, @product_id, 'OUT', @quantity, 'POS_TICKET', CONVERT(NVARCHAR(50), @ticket_id));

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_pay_pos_ticket]
    @company_id INT,
    @ticket_id BIGINT,
    @method NVARCHAR(20),
    @amount DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    -- Validación: método de pago debe estar habilitado para la compañía (si existe catálogo).
    IF EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'config')
    BEGIN
        IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[config].[PaymentMethod]') AND type IN (N'U'))
        BEGIN
            IF NOT EXISTS (
                SELECT 1
                FROM [config].[PaymentMethod] pm WITH (NOLOCK)
                INNER JOIN [config].[CompanyPaymentMethod] cpm WITH (NOLOCK)
                    ON cpm.PaymentMethodId = pm.Id AND cpm.CompanyId = @company_id
                WHERE pm.Active = 1
                  AND cpm.IsEnabled = 1
                  AND LOWER(pm.Code) = LOWER(@method)
            )
            BEGIN
                RAISERROR('Payment method not enabled for company.', 16, 1);
                RETURN;
            END
        END
    END

    INSERT INTO [com].[PosPayment] (CompanyId, TicketId, Method, Amount)
    VALUES (@company_id, @ticket_id, @method, @amount);

    UPDATE [com].[PosTicket]
    SET Status='paid'
    WHERE CompanyId=@company_id AND Id=@ticket_id;

    -- Contabilización PUC mínima (opcional): Caja vs Ventas
    DECLARE @acc_cash BIGINT = NULL;
    DECLARE @acc_sales BIGINT = NULL;

    SELECT TOP 1 @acc_cash = a.Id
    FROM [fin].[Account] a WITH (NOLOCK)
    WHERE a.CompanyId = @company_id AND a.Code = '110505' AND a.Active = 1;

    SELECT TOP 1 @acc_sales = a.Id
    FROM [fin].[Account] a WITH (NOLOCK)
    WHERE a.CompanyId = @company_id AND a.Code = '413501' AND a.Active = 1;

    IF @acc_cash IS NOT NULL AND @acc_sales IS NOT NULL
    BEGIN
        INSERT INTO [fin].[JournalEntry]
            (CompanyId, EntryDate, EntryType, ReferenceType, ReferenceId, [Description], [Status], CreatedByUserId, CreatedAt, UpdatedAt)
        VALUES
            (@company_id, SYSUTCDATETIME(), 'POS', 'POS_TICKET', CAST(@ticket_id AS NVARCHAR(100)), 'POS sale', 'posted', NULL, SYSUTCDATETIME(), SYSUTCDATETIME());

        DECLARE @je_id BIGINT = CAST(SCOPE_IDENTITY() AS BIGINT);

        INSERT INTO [fin].[JournalEntryLine]
            (CompanyId, JournalEntryId, AccountId, Debit, Credit, ThirdPartyDocument, ThirdPartyName, Notes, CreatedAt)
        VALUES
            (@company_id, @je_id, @acc_cash, @amount, 0, NULL, NULL, 'Cash/Bank', SYSUTCDATETIME()),
            (@company_id, @je_id, @acc_sales, 0, @amount, NULL, NULL, 'Sales', SYSUTCDATETIME());
    END

    SELECT CAST(1 AS BIT) AS Success;
END
GO

/* =========================================================
   COMMERCE (Proveedores, Variantes, Compras / Ingreso Inventario)
   ========================================================= */

CREATE OR ALTER PROCEDURE [com].[sp_upsert_supplier]
(
    @company_id INT,
    @id BIGINT = NULL,
    @name NVARCHAR(200),
    @document_number NVARCHAR(50) = NULL,
    @phone NVARCHAR(20) = NULL,
    @email NVARCHAR(150) = NULL,
    @notes NVARCHAR(500) = NULL,
    @active BIT = 1,
    @performed_by_user_id INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @id IS NULL OR @id = 0
    BEGIN
        INSERT INTO [com].[Supplier]
            (CompanyId, Name, DocumentNumber, Phone, Email, Notes, Active, CreatedAt, CreatedByUserId, UpdatedAt, UpdatedByUserId)
        VALUES
            (@company_id, @name, @document_number, @phone, @email, @notes, @active, SYSUTCDATETIME(), @performed_by_user_id, SYSUTCDATETIME(), @performed_by_user_id);

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
        RETURN;
    END

    UPDATE [com].[Supplier]
    SET Name = @name,
        DocumentNumber = @document_number,
        Phone = @phone,
        Email = @email,
        Notes = @notes,
        Active = @active,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @performed_by_user_id
    WHERE Id = @id AND CompanyId = @company_id;

    SELECT @id AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_list_suppliers]
(
    @company_id INT,
    @only_active BIT = 1,
    @q NVARCHAR(100) = NULL,
    @page INT = 1,
    @page_size INT = 50
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@page - 1) * @page_size;

    SELECT
        s.Id,
        s.CompanyId,
        s.Name,
        s.DocumentNumber,
        s.Phone,
        s.Email,
        s.Notes,
        s.Active,
        s.CreatedAt,
        s.UpdatedAt
    FROM [com].[Supplier] s WITH (NOLOCK)
    WHERE s.CompanyId = @company_id
      AND (@only_active = 0 OR s.Active = 1)
      AND (
          @q IS NULL OR s.Name LIKE '%' + @q + '%'
          OR s.DocumentNumber LIKE '%' + @q + '%'
      )
    ORDER BY s.Name
    OFFSET @Offset ROWS FETCH NEXT @page_size ROWS ONLY;

    SELECT COUNT(1) AS Total
    FROM [com].[Supplier] s WITH (NOLOCK)
    WHERE s.CompanyId = @company_id
      AND (@only_active = 0 OR s.Active = 1)
      AND (
          @q IS NULL OR s.Name LIKE '%' + @q + '%'
          OR s.DocumentNumber LIKE '%' + @q + '%'
      );
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_upsert_product_variant]
(
    @company_id INT,
    @id BIGINT = NULL,
    @product_id INT,
    @sku NVARCHAR(80),
    @barcode NVARCHAR(50) = NULL,
    @variant_name NVARCHAR(200) = NULL,
    @price_override DECIMAL(18,2) = NULL,
    @cost_override DECIMAL(18,2) = NULL,
    @active BIT = 1,
    @performed_by_user_id INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @id IS NULL OR @id = 0
    BEGIN
        INSERT INTO [com].[ProductVariant]
            (CompanyId, ProductId, Sku, Barcode, VariantName, PriceOverride, CostOverride, Active, CreatedAt, CreatedByUserId, UpdatedAt, UpdatedByUserId)
        VALUES
            (@company_id, @product_id, @sku, @barcode, @variant_name, @price_override, @cost_override, @active, SYSUTCDATETIME(), @performed_by_user_id, SYSUTCDATETIME(), @performed_by_user_id);

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
        RETURN;
    END

    UPDATE [com].[ProductVariant]
    SET ProductId = @product_id,
        Sku = @sku,
        Barcode = @barcode,
        VariantName = @variant_name,
        PriceOverride = @price_override,
        CostOverride = @cost_override,
        Active = @active,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @performed_by_user_id
    WHERE Id = @id AND CompanyId = @company_id;

    SELECT @id AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_list_product_variants]
(
    @company_id INT,
    @product_id INT = NULL,
    @only_active BIT = 1,
    @q NVARCHAR(100) = NULL,
    @page INT = 1,
    @page_size INT = 50
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@page - 1) * @page_size;

    SELECT
        v.Id,
        v.CompanyId,
        v.ProductId,
        p.Sku AS ProductSku,
        p.Name AS ProductName,
        v.Sku,
        v.Barcode,
        v.VariantName,
        v.PriceOverride,
        v.CostOverride,
        v.Active
    FROM [com].[ProductVariant] v WITH (NOLOCK)
    INNER JOIN [com].[Product] p WITH (NOLOCK) ON p.Id = v.ProductId AND p.CompanyId = v.CompanyId
    WHERE v.CompanyId = @company_id
      AND (@product_id IS NULL OR v.ProductId = @product_id)
      AND (@only_active = 0 OR v.Active = 1)
      AND (
          @q IS NULL OR v.Sku LIKE '%' + @q + '%'
          OR v.Barcode LIKE '%' + @q + '%'
          OR v.VariantName LIKE '%' + @q + '%'
      )
    ORDER BY v.Sku
    OFFSET @Offset ROWS FETCH NEXT @page_size ROWS ONLY;

    SELECT COUNT(1) AS Total
    FROM [com].[ProductVariant] v WITH (NOLOCK)
    WHERE v.CompanyId = @company_id
      AND (@product_id IS NULL OR v.ProductId = @product_id)
      AND (@only_active = 0 OR v.Active = 1)
      AND (
          @q IS NULL OR v.Sku LIKE '%' + @q + '%'
          OR v.Barcode LIKE '%' + @q + '%'
          OR v.VariantName LIKE '%' + @q + '%'
      );
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_create_purchase_receipt]
(
    @company_id INT,
    @supplier_id BIGINT = NULL,
    @receipt_date DATETIME2,
    @supplier_invoice_number NVARCHAR(50) = NULL,
    @default_to_location_id BIGINT = NULL,
    @created_by_user_id INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [com].[PurchaseReceipt]
        (CompanyId, SupplierId, ReceiptDate, SupplierInvoiceNumber, [Status], Total, DefaultToLocationId, CreatedByUserId, CreatedAt, UpdatedAt)
    VALUES
        (@company_id, @supplier_id, @receipt_date, @supplier_invoice_number, 'draft', 0, @default_to_location_id, @created_by_user_id, SYSUTCDATETIME(), SYSUTCDATETIME());

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_add_purchase_receipt_line]
(
    @company_id INT,
    @receipt_id BIGINT,
    @product_id INT,
    @variant_id BIGINT = NULL,
    @quantity DECIMAL(18,4),
    @unit_cost DECIMAL(18,2),
    @to_location_id BIGINT = NULL,
    @notes NVARCHAR(500) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1 FROM [com].[PurchaseReceipt] r WITH (NOLOCK)
        WHERE r.Id = @receipt_id AND r.CompanyId = @company_id AND LOWER(r.[Status]) = 'draft'
    )
    BEGIN
        RAISERROR('PurchaseReceipt not found or not in draft status.', 16, 1);
        RETURN;
    END

    DECLARE @line_total DECIMAL(18,2) = CAST(@quantity * @unit_cost AS DECIMAL(18,2));

    INSERT INTO [com].[PurchaseReceiptLine]
        (CompanyId, ReceiptId, ProductId, VariantId, Quantity, UnitCost, LineTotal, ToLocationId, Notes, CreatedAt)
    VALUES
        (@company_id, @receipt_id, @product_id, @variant_id, @quantity, @unit_cost, @line_total, @to_location_id, @notes, SYSUTCDATETIME());

    UPDATE [com].[PurchaseReceipt]
    SET Total = (SELECT ISNULL(SUM(LineTotal),0) FROM [com].[PurchaseReceiptLine] WHERE CompanyId = @company_id AND ReceiptId = @receipt_id),
        UpdatedAt = SYSUTCDATETIME()
    WHERE CompanyId = @company_id AND Id = @receipt_id;

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [com].[sp_post_purchase_receipt]
(
    @company_id INT,
    @receipt_id BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1 FROM [com].[PurchaseReceipt] r WITH (NOLOCK)
        WHERE r.Id = @receipt_id AND r.CompanyId = @company_id AND LOWER(r.[Status]) = 'draft'
    )
    BEGIN
        RAISERROR('PurchaseReceipt not found or not in draft status.', 16, 1);
        RETURN;
    END

    DECLARE @receipt_date DATETIME2;
    DECLARE @default_loc BIGINT;
    SELECT @receipt_date = ReceiptDate, @default_loc = DefaultToLocationId
    FROM [com].[PurchaseReceipt]
    WHERE CompanyId = @company_id AND Id = @receipt_id;

    -- Insert inventory movements (IN) per line
    INSERT INTO [com].[InventoryMovement]
        (CompanyId, ProductId, VariantId, FromLocationId, ToLocationId, MoveDate, [Type], Quantity, ReferenceType, ReferenceId, Notes)
    SELECT
        l.CompanyId,
        l.ProductId,
        l.VariantId,
        NULL,
        COALESCE(l.ToLocationId, @default_loc),
        @receipt_date,
        'IN',
        l.Quantity,
        'PURCHASE_RECEIPT',
        CAST(@receipt_id AS NVARCHAR(100)),
        l.Notes
    FROM [com].[PurchaseReceiptLine] l
    WHERE l.CompanyId = @company_id AND l.ReceiptId = @receipt_id;

    UPDATE [com].[PurchaseReceipt]
    SET [Status] = 'posted', UpdatedAt = SYSUTCDATETIME()
    WHERE CompanyId = @company_id AND Id = @receipt_id;

    -- Contabilización PUC mínima (opcional): Inventario vs Proveedores
    DECLARE @acc_inv BIGINT = NULL;
    DECLARE @acc_ap BIGINT = NULL;
    DECLARE @total DECIMAL(18,2) = 0;
    DECLARE @supplier_id BIGINT = NULL;

    SELECT @total = Total, @supplier_id = SupplierId
    FROM [com].[PurchaseReceipt]
    WHERE CompanyId = @company_id AND Id = @receipt_id;

    SELECT TOP 1 @acc_inv = a.Id
    FROM [fin].[Account] a WITH (NOLOCK)
    WHERE a.CompanyId = @company_id AND a.Code = '143505' AND a.Active = 1;

    SELECT TOP 1 @acc_ap = a.Id
    FROM [fin].[Account] a WITH (NOLOCK)
    WHERE a.CompanyId = @company_id AND a.Code = '220505' AND a.Active = 1;

    IF @acc_inv IS NOT NULL AND @acc_ap IS NOT NULL AND @total > 0
    BEGIN
        INSERT INTO [fin].[JournalEntry]
            (CompanyId, EntryDate, EntryType, ReferenceType, ReferenceId, [Description], [Status], CreatedByUserId, CreatedAt, UpdatedAt)
        VALUES
            (@company_id, SYSUTCDATETIME(), 'PURCHASE', 'PURCHASE_RECEIPT', CAST(@receipt_id AS NVARCHAR(100)), 'Purchase receipt', 'posted', NULL, SYSUTCDATETIME(), SYSUTCDATETIME());

        DECLARE @je_id2 BIGINT = CAST(SCOPE_IDENTITY() AS BIGINT);

        INSERT INTO [fin].[JournalEntryLine]
            (CompanyId, JournalEntryId, AccountId, Debit, Credit, ThirdPartyDocument, ThirdPartyName, Notes, CreatedAt)
        VALUES
            (@company_id, @je_id2, @acc_inv, @total, 0, NULL, NULL, 'Inventory', SYSUTCDATETIME()),
            (@company_id, @je_id2, @acc_ap, 0, @total, NULL, NULL, 'Accounts payable', SYSUTCDATETIME());
    END

    SELECT CAST(1 AS BIT) AS Success;
END
GO

/* =========================================================
   MFG (Recetas / Producción) - Restaurante / Transformación
   ========================================================= */

CREATE OR ALTER PROCEDURE [mfg].[sp_upsert_recipe]
(
    @company_id INT,
    @id BIGINT = NULL,
    @output_product_id INT,
    @output_variant_id BIGINT = NULL,
    @output_quantity DECIMAL(18,4),
    @notes NVARCHAR(500) = NULL,
    @active BIT = 1
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @id IS NULL OR @id = 0
    BEGIN
        INSERT INTO [mfg].[Recipe]
            (CompanyId, OutputProductId, OutputVariantId, OutputQuantity, Notes, Active, CreatedAt, UpdatedAt)
        VALUES
            (@company_id, @output_product_id, @output_variant_id, @output_quantity, @notes, @active, SYSUTCDATETIME(), SYSUTCDATETIME());

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
        RETURN;
    END

    UPDATE [mfg].[Recipe]
    SET OutputProductId = @output_product_id,
        OutputVariantId = @output_variant_id,
        OutputQuantity = @output_quantity,
        Notes = @notes,
        Active = @active,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @id AND CompanyId = @company_id;

    SELECT @id AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [mfg].[sp_set_recipe_items]
(
    @company_id INT,
    @recipe_id BIGINT,
    @items_json NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Reemplazo completo (simple y consistente)
    DELETE FROM [mfg].[RecipeItem]
    WHERE CompanyId = @company_id AND RecipeId = @recipe_id;

    INSERT INTO [mfg].[RecipeItem]
        (CompanyId, RecipeId, InputProductId, InputVariantId, Quantity, Unit, Notes, CreatedAt)
    SELECT
        @company_id,
        @recipe_id,
        CAST(JSON_VALUE(j.value, '$.inputProductId') AS INT),
        TRY_CAST(JSON_VALUE(j.value, '$.inputVariantId') AS BIGINT),
        CAST(JSON_VALUE(j.value, '$.quantity') AS DECIMAL(18,4)),
        JSON_VALUE(j.value, '$.unit'),
        JSON_VALUE(j.value, '$.notes'),
        SYSUTCDATETIME()
    FROM OPENJSON(@items_json) j;

    SELECT CAST(1 AS BIT) AS Success;
END
GO

CREATE OR ALTER PROCEDURE [mfg].[sp_create_production_batch]
(
    @company_id INT,
    @recipe_id BIGINT = NULL,
    @output_product_id INT,
    @output_variant_id BIGINT = NULL,
    @quantity_produced DECIMAL(18,4),
    @to_location_id BIGINT = NULL,
    @batch_date DATETIME2,
    @notes NVARCHAR(500) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [mfg].[ProductionBatch]
        (CompanyId, RecipeId, OutputProductId, OutputVariantId, QuantityProduced, ToLocationId, BatchDate, [Status], Notes, CreatedAt, UpdatedAt)
    VALUES
        (@company_id, @recipe_id, @output_product_id, @output_variant_id, @quantity_produced, @to_location_id, @batch_date, 'draft', @notes, SYSUTCDATETIME(), SYSUTCDATETIME());

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [mfg].[sp_post_production_batch]
(
    @company_id INT,
    @batch_id BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [mfg].[ProductionBatch] b WHERE b.CompanyId = @company_id AND b.Id = @batch_id AND LOWER(b.[Status]) = 'draft')
    BEGIN
        RAISERROR('ProductionBatch not found or not in draft status.', 16, 1);
        RETURN;
    END

    DECLARE @recipe_id BIGINT;
    DECLARE @output_product_id INT;
    DECLARE @output_variant_id BIGINT;
    DECLARE @qty DECIMAL(18,4);
    DECLARE @to_loc BIGINT;
    DECLARE @batch_date DATETIME2;

    SELECT
        @recipe_id = RecipeId,
        @output_product_id = OutputProductId,
        @output_variant_id = OutputVariantId,
        @qty = QuantityProduced,
        @to_loc = ToLocationId,
        @batch_date = BatchDate
    FROM [mfg].[ProductionBatch]
    WHERE CompanyId = @company_id AND Id = @batch_id;

    IF @recipe_id IS NULL
    BEGIN
        RAISERROR('RecipeId is required to post batch.', 16, 1);
        RETURN;
    END

    -- Consumir insumos según receta: OUT
    INSERT INTO [com].[InventoryMovement]
        (CompanyId, ProductId, VariantId, FromLocationId, ToLocationId, MoveDate, [Type], Quantity, ReferenceType, ReferenceId, Notes)
    SELECT
        @company_id,
        ri.InputProductId,
        ri.InputVariantId,
        @to_loc, -- asumimos consumo desde la misma ubicación destino (si existe); si no, NULL
        NULL,
        @batch_date,
        'OUT',
        (ri.Quantity * @qty),
        'PROD_BATCH',
        CAST(@batch_id AS NVARCHAR(100)),
        ISNULL(ri.Notes, 'Recipe consumption')
    FROM [mfg].[RecipeItem] ri
    WHERE ri.CompanyId = @company_id AND ri.RecipeId = @recipe_id;

    -- Producir terminado: IN
    INSERT INTO [com].[InventoryMovement]
        (CompanyId, ProductId, VariantId, FromLocationId, ToLocationId, MoveDate, [Type], Quantity, ReferenceType, ReferenceId, Notes)
    VALUES
        (@company_id, @output_product_id, @output_variant_id, NULL, @to_loc, @batch_date, 'IN', @qty, 'PROD_BATCH', CAST(@batch_id AS NVARCHAR(100)), 'Production output');

    UPDATE [mfg].[ProductionBatch]
    SET [Status] = 'posted', UpdatedAt = SYSUTCDATETIME()
    WHERE CompanyId = @company_id AND Id = @batch_id;

    SELECT CAST(1 AS BIT) AS Success;
END
GO

CREATE OR ALTER PROCEDURE [mfg].[sp_report_recipe_cost]
(
    @company_id INT,
    @recipe_id BIGINT,
    @quantity DECIMAL(18,4) = 1
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Costo estimado: usa Product.Cost si existe; si no, 0 (puedes refinar con promedio por compras luego)
    SELECT
        ri.InputProductId,
        p.Sku,
        p.Name AS ProductName,
        ri.Quantity * @quantity AS Qty,
        ISNULL(p.Cost, 0) AS UnitCost,
        (ri.Quantity * @quantity) * ISNULL(p.Cost, 0) AS LineCost
    FROM [mfg].[RecipeItem] ri
    INNER JOIN [com].[Product] p ON p.Id = ri.InputProductId AND p.CompanyId = @company_id
    WHERE ri.CompanyId = @company_id AND ri.RecipeId = @recipe_id
    ORDER BY p.Sku;

    SELECT SUM((ri.Quantity * @quantity) * ISNULL(p.Cost, 0)) AS TotalCost
    FROM [mfg].[RecipeItem] ri
    INNER JOIN [com].[Product] p ON p.Id = ri.InputProductId AND p.CompanyId = @company_id
    WHERE ri.CompanyId = @company_id AND ri.RecipeId = @recipe_id;
END
GO

/* =========================================================
   AGRO (Avicultura / Piscicultura)
   ========================================================= */

CREATE OR ALTER PROCEDURE [agro].[sp_create_lot]
(
    @company_id INT,
    @kind NVARCHAR(20),
    @code NVARCHAR(50),
    @name NVARCHAR(200) = NULL,
    @start_date DATETIME2,
    @initial_units INT,
    @initial_avg_weight_kg DECIMAL(18,4) = NULL,
    @notes NVARCHAR(500) = NULL,
    @created_by_user_id INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [agro].[AgroLot]
        (CompanyId, Kind, Code, Name, StartDate, InitialUnits, CurrentUnits, InitialAvgWeightKg, [Status], Notes, CreatedAt, CreatedByUserId, UpdatedAt, UpdatedByUserId)
    VALUES
        (@company_id, @kind, @code, @name, @start_date, @initial_units, @initial_units, @initial_avg_weight_kg, 'open', @notes, SYSUTCDATETIME(), @created_by_user_id, SYSUTCDATETIME(), @created_by_user_id);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [agro].[sp_list_lots]
(
    @company_id INT,
    @kind NVARCHAR(20) = NULL,
    @only_open BIT = 0,
    @page INT = 1,
    @page_size INT = 50
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@page - 1) * @page_size;

    SELECT
        l.Id, l.CompanyId, l.Kind, l.Code, l.Name, l.StartDate,
        l.InitialUnits, l.CurrentUnits, l.InitialAvgWeightKg, l.[Status], l.Notes,
        l.CreatedAt, l.UpdatedAt
    FROM [agro].[AgroLot] l WITH (NOLOCK)
    WHERE l.CompanyId = @company_id
      AND (@kind IS NULL OR l.Kind = @kind)
      AND (@only_open = 0 OR LOWER(l.[Status]) = 'open')
    ORDER BY l.StartDate DESC, l.Id DESC
    OFFSET @Offset ROWS FETCH NEXT @page_size ROWS ONLY;

    SELECT COUNT(1) AS Total
    FROM [agro].[AgroLot] l WITH (NOLOCK)
    WHERE l.CompanyId = @company_id
      AND (@kind IS NULL OR l.Kind = @kind)
      AND (@only_open = 0 OR LOWER(l.[Status]) = 'open');
END
GO

CREATE OR ALTER PROCEDURE [agro].[sp_create_feed_log]
(
    @company_id INT,
    @lot_id BIGINT,
    @feed_date DATETIME2,
    @feed_product_id INT,
    @feed_variant_id BIGINT = NULL,
    @quantity DECIMAL(18,4),
    @from_location_id BIGINT = NULL,
    @unit_cost DECIMAL(18,2) = NULL,
    @notes NVARCHAR(500) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [agro].[AgroFeedLog]
        (CompanyId, LotId, FeedDate, FeedProductId, FeedVariantId, Quantity, FromLocationId, UnitCost, Notes, CreatedAt)
    VALUES
        (@company_id, @lot_id, @feed_date, @feed_product_id, @feed_variant_id, @quantity, @from_location_id, @unit_cost, @notes, SYSUTCDATETIME());

    -- Consume inventory (OUT) of feed
    INSERT INTO [com].[InventoryMovement]
        (CompanyId, ProductId, VariantId, FromLocationId, ToLocationId, MoveDate, [Type], Quantity, ReferenceType, ReferenceId, Notes)
    VALUES
        (@company_id, @feed_product_id, @feed_variant_id, @from_location_id, NULL, @feed_date, 'OUT', @quantity, 'AGRO_FEED', CAST(SCOPE_IDENTITY() AS NVARCHAR(100)), @notes);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [agro].[sp_create_mortality_log]
(
    @company_id INT,
    @lot_id BIGINT,
    @mortality_date DATETIME2,
    @units INT,
    @avg_weight_kg DECIMAL(18,4) = NULL,
    @notes NVARCHAR(500) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [agro].[AgroMortalityLog]
        (CompanyId, LotId, MortalityDate, Units, AvgWeightKg, Notes, CreatedAt)
    VALUES
        (@company_id, @lot_id, @mortality_date, @units, @avg_weight_kg, @notes, SYSUTCDATETIME());

    UPDATE [agro].[AgroLot]
    SET CurrentUnits = CASE WHEN CurrentUnits - @units < 0 THEN 0 ELSE CurrentUnits - @units END,
        UpdatedAt = SYSUTCDATETIME()
    WHERE CompanyId = @company_id AND Id = @lot_id;

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [agro].[sp_create_harvest]
(
    @company_id INT,
    @lot_id BIGINT,
    @harvest_date DATETIME2,
    @output_product_id INT,
    @output_variant_id BIGINT = NULL,
    @units INT,
    @total_weight_kg DECIMAL(18,4) = NULL,
    @to_location_id BIGINT = NULL,
    @notes NVARCHAR(500) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [agro].[AgroHarvest]
        (CompanyId, LotId, HarvestDate, OutputProductId, OutputVariantId, Units, TotalWeightKg, ToLocationId, Notes, CreatedAt)
    VALUES
        (@company_id, @lot_id, @harvest_date, @output_product_id, @output_variant_id, @units, @total_weight_kg, @to_location_id, @notes, SYSUTCDATETIME());

    -- Produce inventory (IN) of output
    INSERT INTO [com].[InventoryMovement]
        (CompanyId, ProductId, VariantId, FromLocationId, ToLocationId, MoveDate, [Type], Quantity, ReferenceType, ReferenceId, Notes)
    VALUES
        (@company_id, @output_product_id, @output_variant_id, NULL, @to_location_id, @harvest_date, 'IN', @units, 'AGRO_HARVEST', CAST(SCOPE_IDENTITY() AS NVARCHAR(100)), @notes);

    UPDATE [agro].[AgroLot]
    SET [Status] = 'closed', UpdatedAt = SYSUTCDATETIME()
    WHERE CompanyId = @company_id AND Id = @lot_id;

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [agro].[sp_report_lot_kpis]
(
    @company_id INT,
    @lot_id BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @initial_units INT;
    DECLARE @current_units INT;
    SELECT @initial_units = InitialUnits, @current_units = CurrentUnits
    FROM [agro].[AgroLot] WITH (NOLOCK)
    WHERE CompanyId = @company_id AND Id = @lot_id;

    SELECT
        @lot_id AS LotId,
        @initial_units AS InitialUnits,
        @current_units AS CurrentUnits,
        (@initial_units - @current_units) AS MortalityUnits,
        CASE WHEN @initial_units = 0 THEN 0 ELSE CAST((@initial_units - @current_units) AS DECIMAL(18,4)) / @initial_units END AS MortalityRate,
        (SELECT ISNULL(SUM(Quantity),0) FROM [agro].[AgroFeedLog] WITH (NOLOCK) WHERE CompanyId = @company_id AND LotId = @lot_id) AS FeedQty,
        (SELECT ISNULL(SUM(Quantity * ISNULL(UnitCost,0)),0) FROM [agro].[AgroFeedLog] WITH (NOLOCK) WHERE CompanyId = @company_id AND LotId = @lot_id) AS FeedCost;
END
GO

