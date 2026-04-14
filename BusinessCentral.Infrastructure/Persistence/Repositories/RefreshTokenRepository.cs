using Microsoft.EntityFrameworkCore;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Domain.Entities.Audit;

namespace BusinessCentral.Infrastructure.Persistence.Adapters
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly BusinessCentralDbContext _context;

        public RefreshTokenRepository(BusinessCentralDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetActiveByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow);
        }

        public async Task RevokeAsync(RefreshToken token, string? replacedByToken = null)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.ReplacedByToken = replacedByToken;
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }

        // Revoke all active tokens for a user (used before issuing a new refresh token)
        public async Task RevokeAllByUserAsync(int userId, string? replacedByToken = null)
        {
            var activeTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            if (!activeTokens.Any())
                return;

            foreach (var tk in activeTokens)
            {
                tk.RevokedAt = DateTime.UtcNow;
                tk.ReplacedByToken = replacedByToken;
            }

            _context.RefreshTokens.UpdateRange(activeTokens);
            await _context.SaveChangesAsync();
        }

        // Revoke all active tokens for a company (works if tokens have CompanyId snapshot filled)
        public async Task RevokeAllByCompanyAsync(int companyId, string? replacedByToken = null)
        {
            var activeTokens = await _context.RefreshTokens
                .Where(rt => rt.CompanyId == companyId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            if (!activeTokens.Any())
                return;

            foreach (var tk in activeTokens)
            {
                tk.RevokedAt = DateTime.UtcNow;
                tk.ReplacedByToken = replacedByToken;
            }

            _context.RefreshTokens.UpdateRange(activeTokens);
            await _context.SaveChangesAsync();
        }
    }
}