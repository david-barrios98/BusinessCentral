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