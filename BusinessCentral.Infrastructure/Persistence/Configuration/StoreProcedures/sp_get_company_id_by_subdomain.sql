/*
  Devuelve el CompanyId (tenant) a partir del subdominio.

  Recomendación:
  - `business.Companies.Subdomain` debe ser único.
  - Normaliza a lower-case al guardar para evitar problemas.
*/
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

