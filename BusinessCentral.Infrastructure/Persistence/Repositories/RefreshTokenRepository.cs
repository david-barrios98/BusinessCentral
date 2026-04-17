using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Domain.Entities.Audit;
using BusinessCentral.Infrastructure.Persistence.Adapters;
using BusinessCentral.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BusinessCentral.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly BusinessCentralDbContext _context;
        private readonly DateTime _getColombiaTimeNow;

        public RefreshTokenRepository(BusinessCentralDbContext context)
        {
            _context = context;
            _getColombiaTimeNow = TimeZoneHelper.GetColombiaTimeNow();
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
                .FirstOrDefaultAsync(rt => rt.Token == token && rt.RevokedAt == null && rt.ExpiresAt > _getColombiaTimeNow);
        }

        public async Task RevokeAsync(RefreshToken token, string? replacedByToken = null)
        {
            token.RevokedAt = TimeZoneHelper.GetColombiaTimeNow();
            token.ReplacedByToken = replacedByToken;
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }

        // Revoke all active tokens for a user (used before issuing a new refresh token)
        public async Task RevokeAllByUserAsync(int userId, string? replacedByToken = null)
        {
            var activeTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > _getColombiaTimeNow)
                .ToListAsync();

            if (!activeTokens.Any())
                return;

            foreach (var tk in activeTokens)
            {
                tk.RevokedAt = _getColombiaTimeNow;
                tk.ReplacedByToken = replacedByToken;
            }

            _context.RefreshTokens.UpdateRange(activeTokens);
            await _context.SaveChangesAsync();
        }

        // Revoke all active tokens for a company (works if tokens have CompanyId snapshot filled)
        public async Task RevokeAllByCompanyAsync(int companyId, string? replacedByToken = null)
        {
            var activeTokens = await _context.RefreshTokens
                .Where(rt => rt.CompanyId == companyId && rt.RevokedAt == null && rt.ExpiresAt > _getColombiaTimeNow)
                .ToListAsync();

            if (!activeTokens.Any())
                return;

            foreach (var tk in activeTokens)
            {
                tk.RevokedAt = _getColombiaTimeNow;
                tk.ReplacedByToken = replacedByToken;
            }

            _context.RefreshTokens.UpdateRange(activeTokens);
            await _context.SaveChangesAsync();
        }
    }
}