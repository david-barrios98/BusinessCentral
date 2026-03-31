CREATE OR ALTER PROCEDURE config.sp_check_tenant_access
    @company_id INT,
    @module_name VARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CurrentDate DATETIME = GETUTCDATE();
    DECLARE @Result VARCHAR(50);

    -- 1. Verificar si la compañía existe y está activa
    IF NOT EXISTS (SELECT 1 FROM auth.companies WHERE id = @company_id AND is_active = 1)
BEGIN
SELECT 'CompanyDisabled' AS AccessStatus;
RETURN;
END

    -- 2. Verificar si tiene una suscripción vigente
    DECLARE @PlanId INT;
SELECT @PlanId = plan_id
FROM config.company_subscriptions
WHERE company_id = @company_id
  AND is_active = 1
  AND @CurrentDate BETWEEN start_date AND end_date;

IF @PlanId IS NULL
BEGIN
SELECT 'SubscriptionExpired' AS AccessStatus;
RETURN;
END

    -- 3. Si se solicita un módulo, verificar si el plan o la empresa lo tienen
    IF @module_name IS NOT NULL
BEGIN
        IF NOT EXISTS (
            -- Módulos del Plan
            SELECT 1 FROM config.plan_modules pm
            INNER JOIN auth.modules m ON pm.module_id = m.id
            WHERE pm.plan_id = @PlanId AND m.code = @module_name
        ) AND NOT EXISTS (
            -- Módulos personalizados (Add-ons)
            SELECT 1 FROM auth.company_custom_modules ccm
            INNER JOIN auth.modules m ON ccm.module_id = m.id
            WHERE ccm.company_id = @company_id AND m.code = @module_name AND ccm.is_enabled = 1
        )
BEGIN
SELECT 'ModuleNotIncluded' AS AccessStatus;
RETURN;
END
END

    -- 4. Si pasó todos los filtros
SELECT 'Success' AS AccessStatus;
END