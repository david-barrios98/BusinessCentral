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

