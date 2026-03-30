CREATE OR ALTER PROCEDURE [auth].[sp_login_user]
(
    @username VARCHAR(150)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        u.id as user_id,
        u.username,
        u.password
    FROM [auth].[users] u WITH (NOLOCK)
    WHERE
        u.active = 1
        AND u.username = @username

    -- ============================================
    -- ⚠️ NO ENCONTRADO
    -- ============================================
    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Usuario no encontrado con la configuración de login definida.', 16, 1);
    END
END
GO