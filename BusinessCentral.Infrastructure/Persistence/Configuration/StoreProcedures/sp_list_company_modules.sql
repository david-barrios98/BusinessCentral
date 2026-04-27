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

