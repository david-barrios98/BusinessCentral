CREATE OR ALTER PROCEDURE [config].[sp_check_tenant_access]
    @company_id INT,
    @module_name VARCHAR(50) = NULL -- Puede ser el Code del módulo
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CurrentDate DATETIME = GETUTCDATE();

    -- 1. Verificar si la compañía existe y está activa
    IF NOT EXISTS (SELECT 1 FROM [business].[Companies] WITH (NOLOCK) WHERE Id = @company_id AND Active = 1)
BEGIN
SELECT 'CompanyDisabled' AS AccessStatus;
RETURN;
END

    -- 2. Verificar si tiene una suscripción vigente y activa
    -- Nota: Usamos el campo Status de tu tabla CompanySubscription
    DECLARE @PlanId INT;
SELECT @PlanId = MembershipPlanId
FROM [config].[CompanySubscription] WITH (NOLOCK)
WHERE CompanyId = @company_id
  AND @CurrentDate BETWEEN StartDate AND EndDate
  AND [Status] = 'Active'; -- Ajustar según los valores que manejes

IF @PlanId IS NULL
BEGIN
SELECT 'SubscriptionExpiredOrInactive' AS AccessStatus;
RETURN;
END

    -- 3. Si se solicita un módulo específico, validar que esté en el plan
    IF @module_name IS NOT NULL
BEGIN
        IF NOT EXISTS (
            SELECT 1 
            FROM [config].[PlanModule] pm WITH (NOLOCK) 
            INNER JOIN [config].[Module] m WITH (NOLOCK) ON pm.ModuleId = m.Id
            WHERE pm.MembershipPlanId = @PlanId 
              AND (m.Code = @module_name OR m.Name = @module_name)
              AND m.Active = 1
        )
BEGIN
SELECT 'ModuleNotIncluded' AS AccessStatus;
RETURN;
END
END

    -- 4. Acceso concedido
SELECT 'Success' AS AccessStatus;
END
GO