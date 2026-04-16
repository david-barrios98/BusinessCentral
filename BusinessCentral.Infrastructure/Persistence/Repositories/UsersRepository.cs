using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace BusinessCentral.Infrastructure.Persistence.Adapters
{
    public class UsersRepository : SqlConfigServer, IUserRepository
    {
        public UsersRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<int?> CreateUserAsync(CreateUserDTO dto)
        {
            var parameters = new[]
            {
                CreateParameter("@CompanyId", dto.CompanyId, SqlDbType.Int),
                CreateParameter("@DocumentTypeId", dto.DocumentTypeId, SqlDbType.Int),
                CreateParameter("@DocumentNumber", dto.DocumentNumber ?? string.Empty, SqlDbType.VarChar),
                CreateParameter("@FirstName", dto.FirstName, SqlDbType.VarChar),
                CreateParameter("@LastName", dto.LastName ?? string.Empty, SqlDbType.VarChar),
                CreateParameter("@Email", dto.Email ?? string.Empty, SqlDbType.VarChar),
                CreateParameter("@Phone", dto.Phone, SqlDbType.VarChar),
                CreateParameter("@PasswordHash", dto.Password, SqlDbType.NVarChar),
                CreateParameter("@AuthProvider", dto.AuthProvider, SqlDbType.VarChar),
                CreateParameter("@ExternalId", dto.ExternalId ?? string.Empty, SqlDbType.VarChar),
                CreateParameter("@RoleId", dto.RoleId, SqlDbType.Int)
            };

            var insertedId = await ExecuteStoredProcedureSingleAsync(
                "[auth].[sp_create_user]",
                parameters,
                reader => Convert.ToInt32(reader.GetValue(0)));

            return insertedId;
        }

        public async Task<UserResponseDTO?> GetUserByIdAsync(int userId)
        {
            var parameters = new[]
            {
                CreateParameter("@UserId", userId, SqlDbType.Int)
            };

            return await ExecuteStoredProcedureSingleAsync(
                "[auth].[sp_get_user_by_id]",
                parameters,
                reader => SqlDataReaderMapper.MapToDto<UserResponseDTO>(reader));
        }

        public async Task UpdateUserAsync(UpdateUserDTO dto)
        {
            var parameters = new[]
            {
                CreateParameter("@UserId", dto.UserId, SqlDbType.Int),
                CreateParameter("@DocumentTypeId", dto.DocumentTypeId, SqlDbType.Int),
                CreateParameter("@DocumentNumber", dto.DocumentNumber ?? string.Empty, SqlDbType.VarChar),
                CreateParameter("@FirstName", dto.FirstName, SqlDbType.VarChar),
                CreateParameter("@LastName", dto.LastName ?? string.Empty, SqlDbType.VarChar),
                CreateParameter("@Email", dto.Email ?? string.Empty, SqlDbType.VarChar),
                CreateParameter("@Phone", dto.Phone, SqlDbType.VarChar),
                CreateParameter("@PasswordHash", dto.Password != null ? dto.Password : (object)DBNull.Value, SqlDbType.NVarChar),
                CreateParameter("@AuthProvider", dto.AuthProvider, SqlDbType.VarChar),
                CreateParameter("@ExternalId", dto.ExternalId ?? string.Empty, SqlDbType.VarChar),
                CreateParameter("@RoleId", dto.RoleId, SqlDbType.Int),
                CreateParameter("@Active", dto.Active ? 1 : 0, SqlDbType.Bit)
            };

            await ExecuteStoredProcedureNonQueryAsync("[auth].[sp_update_user]", parameters);
        }

        public async Task DeleteUserAsync(int userId)
        {
            var parameters = new[]
            {
                CreateParameter("@UserId", userId, SqlDbType.Int)
            };

            await ExecuteStoredProcedureNonQueryAsync("[auth].[sp_delete_user]", parameters);
        }

        public async Task<List<UserResponseDTO>> ListUsersAsync(int companyId, int page, int pageSize)
        {
            var parameters = new[]
            {
                CreateParameter("@CompanyId", companyId, SqlDbType.Int),
                CreateParameter("@Page", page, SqlDbType.Int),
                CreateParameter("@PageSize", pageSize, SqlDbType.Int)
            };

            return await ExecuteStoredProcedureAsync(
                "[auth].[sp_list_users]",
                parameters,
                reader => SqlDataReaderMapper.MapToDto<UserResponseDTO>(reader));
        }
    }
}