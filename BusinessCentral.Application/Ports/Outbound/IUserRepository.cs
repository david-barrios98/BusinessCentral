using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.DTOs.Common;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface IUserRepository
    {
        Task<int?> CreateUserAsync(CreateUserDTO dto);
        Task<UserResponseDTO?> GetUserByIdAsync(int userId);
        Task UpdateUserAsync(int companyId, UpdateUserDTO dto);
        Task DeleteUserAsync(int companyId, int userId);
        Task<PagedResult<UserResponseDTO>> ListUsersAsync(int companyId, int page, int pageSize);
    }
}