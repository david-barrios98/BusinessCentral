namespace BusinessCentral.Api.Services;

public interface ITenantContext
{
    int? CompanyId { get; set; }
    string? Subdomain { get; set; }
    string? CorrelationId { get; set; }
}

public sealed class TenantContext : ITenantContext
{
    public int? CompanyId { get; set; }
    public string? Subdomain { get; set; }
    public string? CorrelationId { get; set; }
}

