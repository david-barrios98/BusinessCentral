namespace BusinessCentral.Application.Ports.Outbound;

public interface ITenantRepository
{
    Task<int?> GetCompanyIdBySubdomainAsync(string subdomain);
}

