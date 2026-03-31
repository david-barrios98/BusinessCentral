CREATE OR ALTER PROCEDURE [auth].[sp_login_user]
(
    @username VARCHAR(150),
    @company_id INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HasPrimary BIT = 0;

    -- ============================================
    -- 🔍 VALIDAR CONFIGURACIÓN DE LOGIN
    -- ============================================
    IF NOT EXISTS (
        SELECT 1
        FROM [config].[application_companies] WITH (NOLOCK)
        WHERE company_id = @company_id
    )
    BEGIN
        RAISERROR('La compañía no tiene configuración de login.', 16, 1);
        RETURN;
    END

    -- ============================================
    -- 🔍 VALIDAR SI EXISTE PRIMARY
    -- ============================================
    IF EXISTS (
        SELECT 1
        FROM [config].[application_companies] WITH (NOLOCK)
        WHERE company_id = @company_id
          AND is_primary = 1
    )
    BEGIN
        SET @HasPrimary = 1;
    END

    -- ============================================
    -- 🔍 QUERY PRINCIPAL CON PRIORIDAD DINÁMICA
    -- ============================================
    ;WITH LoginConfig AS
    (
        SELECT
            type,
            is_primary,
            ROW_NUMBER() OVER (
                ORDER BY 
                    CASE 
                        WHEN is_primary = 1 THEN 1
                        WHEN @HasPrimary = 0 AND type = 'phone' THEN 1
                        ELSE 2
                    END
            ) AS priority
        FROM [config].[application_companies] WITH (NOLOCK)
        WHERE company_id = @company_id and active = 1
    )

    SELECT TOP 1
        ui.users_id,
        pd.value AS document_type,
        ui.document,
        ui.phone,
        ui.email,
        ui.first_name,
        ui.last_name,
        ui.address AS address_users,
        ui.password,
        c.name AS company_name,
        c.id AS company_id
    FROM LoginConfig lc
    INNER JOIN [auth].[users_info] ui WITH (NOLOCK)
        ON u.id = ui.users_id
    INNER JOIN [business].[companies] c WITH (NOLOCK)
        ON u.company_id = c.id and c.active = 1
    WHERE
        ui.active = 1
        AND c.active = 1
        AND (
            (lc.type = 'phone' AND ui.phone = @username)
            OR
            (lc.type = 'email' AND ui.email = @username)
            OR
            (lc.type = 'document' AND ui.document = @username)
        )
    ORDER BY lc.priority;

    -- ============================================
    -- ⚠️ NO ENCONTRADO
    -- ============================================
    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Usuario no encontrado con la configuración de login definida.', 16, 1);
    END
END
GO