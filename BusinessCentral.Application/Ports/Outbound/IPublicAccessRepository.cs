using BusinessCentral.Application.DTOs.Auth;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IPublicAccessRepository
{
    Task<long> CreatePublicTokenAsync(int companyId, int userId, string tokenHashHex, string scope, DateTime expiresAtUtc);
    Task<PublicHrAccountSummaryDTO> GetHrAccountSummaryByTokenAsync(string tokenHashHex);
}

