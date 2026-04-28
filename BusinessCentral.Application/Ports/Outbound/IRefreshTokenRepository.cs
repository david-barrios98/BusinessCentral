using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Domain.Entities.Audit;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<LoginResponseDTO?> GetActiveByTokenAsync(string token);
        Task RevokeAsync(RefreshToken token, string? replacedByToken = null);
        Task RevokeAllByUserAsync(long userSessionId, string? replacedByToken = null);
        Task RevokeAllByUserIdAsync(int userId, string? replacedByToken = null);
        Task RevokeAllByCompanyAsync(int companyId, string? replacedByToken = null);
    }
}