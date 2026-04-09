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