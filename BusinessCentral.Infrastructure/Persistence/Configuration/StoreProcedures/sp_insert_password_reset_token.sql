CREATE OR ALTER PROCEDURE [auth].[sp_insert_password_reset_token]
(
    @UserId INT,
    @Token VARCHAR(255),
    @ExpiresAt DATETIME2,
    @CreatedAt DATETIME2
)
AS
BEGIN
    SET NOCOUNT ON;

INSERT INTO [audit].[PasswordResetToken] (UserId, Token, ExpiresAt, CreatedAt)
VALUES (@UserId, @Token, @ExpiresAt, @CreatedAt);

SELECT SCOPE_IDENTITY() AS InsertedId;
END
GO