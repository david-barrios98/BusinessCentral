using BusinessCentral.Domain.Entities.Audit;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface IUserSessionRepository
    {
        Task<Int64> AddAsync(UserSession session);
        Task UpdateAsync(UserSession session);
        Task<UserSession?> GetByIdAsync(long id);
        Task CloseSessionsByUserAsync(int userId);
        Task CloseSessionsByCompanyAsync(int companyId);
    }
}