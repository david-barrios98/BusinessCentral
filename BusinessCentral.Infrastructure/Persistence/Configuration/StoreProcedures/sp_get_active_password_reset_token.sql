CREATE OR ALTER PROCEDURE [auth].[sp_get_active_password_reset_token]
(
    @Token VARCHAR(255) = NULL,
    @ExpiresAt DATETIME2,
    @UserId INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @UserId IS NOT NULL AND @UserId > 0 AND @Token IS NULL
BEGIN
SELECT pr.Id, pr.UserId, pr.Token, pr.ExpiresAt, pr.CreatedAt, pr.UsedAt,
       ui.Id AS UserId, ui.Email as UserName, ui.Email, ui.FirstName, ui.LastName, ui.CompanyId
FROM [audit].[PasswordResetToken] pr WITH (NOLOCK)
    INNER JOIN [auth].[UsersInfo] ui WITH (NOLOCK) ON pr.UserId = ui.Id
WHERE pr.UserId = @UserId
  AND pr.UsedAt IS NULL
  AND pr.ExpiresAt > @ExpiresAt;
END
ELSE
BEGIN
SELECT pr.Id, pr.UserId, pr.Token, pr.ExpiresAt, pr.CreatedAt, pr.UsedAt,
       ui.Id AS UserId, ui.Email as UserName, ui.Email, ui.FirstName, ui.LastName, ui.CompanyId
FROM [audit].[PasswordResetToken] pr WITH (NOLOCK)
    INNER JOIN [auth].[UsersInfo] ui WITH (NOLOCK) ON pr.UserId = ui.Id
WHERE pr.Token = @Token
  AND pr.UsedAt IS NULL
  AND pr.ExpiresAt > @ExpiresAt;
END

END
GO