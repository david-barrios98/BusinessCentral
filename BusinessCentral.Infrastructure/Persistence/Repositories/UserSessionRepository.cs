using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Domain.Entities.Audit;
using BusinessCentral.Infrastructure.Persistence.Adapters;
using BusinessCentral.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BusinessCentral.Infrastructure.Persistence.Repositories
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

        // Cerrar todas las sesiones activas de un usuario
        public async Task CloseSessionsByUserAsync(int userId)
        {
            var active = await _context.UserSessions
                .Where(s => s.UserId == userId && s.LogoutAt == null && s.IsSuccess)
                .ToListAsync();

            if (!active.Any()) return;

            foreach (var s in active)
            {
                s.LogoutAt = TimeZoneHelper.GetColombiaTimeNow();
                s.IsSuccess = false; // opcional: marcamos como no exitosa por fuerza
            }

            _context.UserSessions.UpdateRange(active);
            await _context.SaveChangesAsync();
        }

        // Cerrar todas las sesiones activas de una company
        public async Task CloseSessionsByCompanyAsync(int companyId)
        {
            var active = await _context.UserSessions
                .Where(s => s.CompanyId == companyId && s.LogoutAt == null && s.IsSuccess)
                .ToListAsync();

            if (!active.Any()) return;

            foreach (var s in active)
            {
                s.LogoutAt = TimeZoneHelper.GetColombiaTimeNow();
                s.IsSuccess = false;
            }

            _context.UserSessions.UpdateRange(active);
            await _context.SaveChangesAsync();
        }
    }
}