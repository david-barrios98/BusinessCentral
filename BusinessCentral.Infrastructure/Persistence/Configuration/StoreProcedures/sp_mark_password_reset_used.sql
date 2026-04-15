CREATE OR ALTER PROCEDURE [auth].[sp_mark_password_reset_used]
(
    @Token VARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

UPDATE [audit].[PasswordResetToken]
SET UsedAt = SYSUTCDATETIME()
WHERE Token = @Token;
END
GO