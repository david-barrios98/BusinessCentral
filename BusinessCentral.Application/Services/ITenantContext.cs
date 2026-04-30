namespace BusinessCentral.Application.Services;

/// <summary>
/// Contexto de tenant (CompanyId) por request.
/// Debe ser Scoped para mantener consistencia durante toda la petición.
/// </summary>
public interface ITenantContext
{
    int? CompanyId { get; set; }
    string? Subdomain { get; set; }
    string? CorrelationId { get; set; }
}

