CREATE OR ALTER   PROCEDURE [auth].[sp_get_rol_user]
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
    INNER JOIN [config].[Role] r WITH (NOLOCK) on ui.RoleId =r.Id
WHERE ui.Id = @UserId;
END