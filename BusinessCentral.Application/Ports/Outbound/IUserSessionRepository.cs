using BusinessCentral.Domain.Entities.Audit;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface IUserSessionRepository
    {
        Task AddAsync(UserSession session);
        Task UpdateAsync(UserSession session);
        Task<UserSession?> GetByIdAsync(long id);
    }
}