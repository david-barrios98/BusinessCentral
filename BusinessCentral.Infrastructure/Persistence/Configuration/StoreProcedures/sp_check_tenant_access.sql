CREATE OR ALTER PROCEDURE config.sp_check_tenant_access
    @company_id INT,
    @module_name VARCHAR(50) = NULL
    AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CurrentDate DATETIME = GETUTCDATE();
    DECLARE @Result VARCHAR(50);

    -- 1. Verificar si la compañía existe y está activa
    IF NOT EXISTS (SELECT 1 FROM business.companies WHERE id = @company_id AND Active = 1)
BEGIN
SELECT 'CompanyDisabled' AS AccessStatus;
RETURN;
END

    -- 2. Verificar si tiene una suscripción vigente
    DECLARE @PlanId INT;
SELECT @PlanId = MembershipPlanId
FROM config.CompanySubscription
WHERE CompanyId = @company_id
  AND @CurrentDate BETWEEN StartDate AND EndDate;

IF @PlanId IS NULL
BEGIN
SELECT 'SubscriptionExpired' AS AccessStatus;
RETURN;
END

SELECT 'ModuleNotIncluded' AS AccessStatus;
RETURN;
END

    -- 4. Si pasó todos los filtros
SELECT 'Success' AS AccessStatus;