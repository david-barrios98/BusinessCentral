CREATE OR ALTER PROCEDURE [auth].[sp_get_active_password_reset_token]
(
    @Token VARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

SELECT pr.Id, pr.UserId, pr.Token, pr.ExpiresAt, pr.CreatedAt, pr.UsedAt,
       ui.Id AS UserId, ui.UserName, ui.Email, ui.FirstName, ui.LastName, ui.CompanyId
FROM [audit].[PasswordResetToken] pr WITH (NOLOCK)
    INNER JOIN [auth].[UsersInfo] ui WITH (NOLOCK) ON pr.UserId = ui.Id
WHERE pr.Token = @Token
  AND pr.UsedAt IS NULL
  AND pr.ExpiresAt > SYSUTCDATETIME();
END
GO