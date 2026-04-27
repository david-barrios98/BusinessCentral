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

