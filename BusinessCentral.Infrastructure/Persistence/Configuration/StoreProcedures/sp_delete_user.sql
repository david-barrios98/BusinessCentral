CREATE OR ALTER PROCEDURE [auth].[sp_delete_user]
(
    @UserId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Soft delete
    UPDATE [auth].[UsersInfo]
    SET Active = 0, Updated = SYSUTCDATETIME()
    WHERE Id = @UserId;
END
GO