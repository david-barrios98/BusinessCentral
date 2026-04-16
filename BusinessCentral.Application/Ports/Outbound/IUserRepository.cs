using BusinessCentral.Application.DTOs.Auth;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface IUserRepository
    {
        Task<int?> CreateUserAsync(CreateUserDTO dto);
        Task<UserResponseDTO?> GetUserByIdAsync(int userId);
        Task UpdateUserAsync(UpdateUserDTO dto);
        Task DeleteUserAsync(int userId);
        Task<List<UserResponseDTO>> ListUsersAsync(int companyId, int page, int pageSize);
    }
}