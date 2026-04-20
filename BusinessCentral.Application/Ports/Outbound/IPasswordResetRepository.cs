using BusinessCentral.Domain.Entities.Audit;
using BusinessCentral.Application.DTOs.Auth;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface IPasswordResetRepository
    {
        Task<PasswordResetToken?> GetActiveByTokenAsync(string? token = null, int? userId = null);
        Task<int?> InsertPasswordResetTokenAsync(int userId, string token);
        Task MarkAsUsedAsync(string token);

        // SP-based user lookup (consistente con login)
        Task<LoginResponseDTO?> GetUserByEmailAndCompanyAsync(string email, int companyId);
        Task UpdateUserPasswordAsync(int userId, string hashedPassword); // llama a sp_update_user_password
    }
}