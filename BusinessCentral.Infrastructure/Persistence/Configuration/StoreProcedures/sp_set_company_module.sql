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

