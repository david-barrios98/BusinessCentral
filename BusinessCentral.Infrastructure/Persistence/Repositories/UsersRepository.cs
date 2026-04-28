using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using BusinessCentral.Infrastructure.Extensions;
using BusinessCentral.Infrastructure.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories
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
                StoredProcedures.User.sp_create_user, 
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
                StoredProcedures.User.sp_get_user_by_id,
                parameters,
                reader => SqlDataReaderMapper.MapToDto<UserResponseDTO>(reader));
        }

        public async Task UpdateUserAsync(int companyId, UpdateUserDTO dto)
        {
            var parameters = new[]
            {
                CreateParameter("@CompanyId", companyId, SqlDbType.Int),
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

            await ExecuteStoredProcedureNonQueryAsync(StoredProcedures.User.sp_update_user, parameters);
        }

        public async Task DeleteUserAsync(int companyId, int userId)
        {
            var parameters = new[]
            {
                CreateParameter("@CompanyId", companyId, SqlDbType.Int),
                CreateParameter("@UserId", userId, SqlDbType.Int)
            };

            await ExecuteStoredProcedureNonQueryAsync(StoredProcedures.User.sp_delete_user, parameters);
        }

        public async Task<PagedResult<UserResponseDTO>> ListUsersAsync(int companyId, int page, int pageSize)
        {
            await using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(StoredProcedures.User.sp_list_users, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add(CreateParameter("@CompanyId", companyId, SqlDbType.Int));
            command.Parameters.Add(CreateParameter("@Page", page, SqlDbType.Int));
            command.Parameters.Add(CreateParameter("@PageSize", pageSize, SqlDbType.Int));

            var items = new List<UserResponseDTO>();
            long total = 0;

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                items.Add(SqlDataReaderMapper.MapToDto<UserResponseDTO>(reader));
            }

            if (await reader.NextResultAsync() && await reader.ReadAsync())
                total = Convert.ToInt64(reader["Total"]);

            return new PagedResult<UserResponseDTO>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }

        public async Task<List<ValidateRolUser>> RolUsersAsync(int userId)
        {
            var parameters = new[]
            {
                CreateParameter("@UserId", userId, SqlDbType.Int)
            };

            return await ExecuteStoredProcedureAsync(
                StoredProcedures.User.sp_get_rol_user,
                parameters,
                reader => SqlDataReaderMapper.MapToDto<ValidateRolUser>(reader));
        }
    }
}