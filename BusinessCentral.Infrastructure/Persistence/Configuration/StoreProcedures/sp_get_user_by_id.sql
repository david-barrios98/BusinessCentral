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