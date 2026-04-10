using Microsoft.EntityFrameworkCore;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Domain.Entities.Audit;

namespace BusinessCentral.Infrastructure.Persistence.Adapters
{
    public class UserSessionRepository : IUserSessionRepository
    {
        private readonly BusinessCentralDbContext _context;

        public UserSessionRepository(BusinessCentralDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserSession session)
        {
            await _context.UserSessions.AddAsync(session);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserSession session)
        {
            _context.UserSessions.Update(session);
            await _context.SaveChangesAsync();
        }

        public async Task<UserSession?> GetByIdAsync(long id)
        {
            return await _context.UserSessions.FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}