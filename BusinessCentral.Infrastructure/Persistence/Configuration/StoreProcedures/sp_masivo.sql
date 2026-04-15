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

    INSERT INTO [audit].[PasswordResetToken] (UserId, Token, ExpiresAt, CreatedAt)
    VALUES (@UserId, @Token, @ExpiresAt, @CreatedAt);

    SELECT SCOPE_IDENTITY() AS InsertedId;
END
GO

CREATE OR ALTER PROCEDURE [auth].[sp_get_active_password_reset_token]
(
    @Token VARCHAR(255) = NULL,
    @ExpiresAt DATETIME2,
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
          AND pr.ExpiresAt > @ExpiresAt;
    END
    ELSE
    BEGIN
        SELECT pr.Id, pr.UserId, pr.Token, pr.ExpiresAt, pr.CreatedAt, pr.UsedAt,
               ui.Id AS UserId, ui.Email as UserName, ui.Email, ui.FirstName, ui.LastName, ui.CompanyId
        FROM [audit].[PasswordResetToken] pr WITH (NOLOCK)
            INNER JOIN [auth].[UsersInfo] ui WITH (NOLOCK) ON pr.UserId = ui.Id
        WHERE pr.Token = @Token
          AND pr.UsedAt IS NULL
          AND pr.ExpiresAt > @ExpiresAt;
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
    SET UsedAt = SYSUTCDATETIME()
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
