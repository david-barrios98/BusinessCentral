using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Domain.Entities.Audit;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> GetActiveByTokenAsync(string token);
        Task RevokeAsync(RefreshToken token, string? replacedByToken = null);
    }
}