namespace BusinessCentral.Application.Constants;

/// <summary>
/// Escenarios de arranque acordados con consultoría: desde cero, negocio sin ERP, migración desde otro sistema.
/// </summary>
public static class FinancialStartupModes
{
    /// <summary>Emprendedor: capital inicial y activos vs patrimonio.</summary>
    public const string Constitution = "CONSTITUTION";

    /// <summary>Ya comercializa sin software: saldos iniciales, cartera, proveedores, inventario.</summary>
    public const string Sanitation = "SANITATION";

    /// <summary>Viene de otro software: saldos de cierre / balance de prueba.</summary>
    public const string Migration = "MIGRATION";

    public static bool IsKnown(string? mode) =>
        mode is Constitution or Sanitation or Migration;
}

/// <summary>Estado del asistente de arranque contable por compañía.</summary>
public static class FinancialBootstrapStatuses
{
    public const string NotStarted = "NOT_STARTED";
    public const string InProgress = "IN_PROGRESS";
    public const string Completed = "COMPLETED";

    public static bool IsKnown(string? status) =>
        status is NotStarted or InProgress or Completed;
}
