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
    }
}