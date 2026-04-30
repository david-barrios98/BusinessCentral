using BusinessCentral.Application.Services;

namespace BusinessCentral.Api.Services;

public sealed class TenantContext : ITenantContext
{
    public int? CompanyId { get; set; }
    public string? Subdomain { get; set; }
    public string? CorrelationId { get; set; }
}

