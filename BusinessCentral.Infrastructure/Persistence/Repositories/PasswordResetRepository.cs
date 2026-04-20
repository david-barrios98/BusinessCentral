using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Domain.Entities.Audit;
using BusinessCentral.Infrastructure.Constants;
using BusinessCentral.Infrastructure.Extensions;
using BusinessCentral.Infrastructure.Helpers;
using BusinessCentral.Infrastructure.Persistence.Adapters;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories
{
    public class PasswordResetRepository : SqlConfigServer, IPasswordResetRepository
    {
        public PasswordResetRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<LoginResponseDTO?> GetUserByEmailAndCompanyAsync(string email, int companyId)
        {
            var parameters = new[]
            {
                CreateParameter("@Email", email, SqlDbType.VarChar),
                CreateParameter("@CompanyId", companyId, SqlDbType.Int)
            };

            return await ExecuteStoredProcedureSingleAsync(
                StoredProcedures.Auth.sp_get_user_by_email_company,
                parameters,
                reader => SqlDataReaderMapper.MapToDto<LoginResponseDTO>(reader));
        }

        public async Task<int?> InsertPasswordResetTokenAsync(int userId, string token)
        {
            var parameters = new[]
            {
                CreateParameter("@UserId", userId, SqlDbType.Int),
                CreateParameter("@Token", token, SqlDbType.NVarChar),
                CreateParameter("@ExpiresAt", TimeZoneHelper.GetColombiaTime().AddHours(2), SqlDbType.DateTime2),
                CreateParameter("@CreatedAt", TimeZoneHelper.GetColombiaTime(), SqlDbType.DateTime2)
            };

            var result = await ExecuteStoredProcedureSingleAsync(
                StoredProcedures.Auth.sp_insert_password_reset_token,
                parameters,
                reader => Convert.ToInt32(reader.GetValue(0)));

            if (result == null) return null;
            return (int)result;
        }

        public async Task<PasswordResetToken?> GetActiveByTokenAsync(string? token = null, int? userId = null)
        {
            var parameters = new[]
            {
                CreateParameter("@Token", token, SqlDbType.NVarChar),
                CreateParameter("@UserId", userId, SqlDbType.Int)

            };
            return await ExecuteStoredProcedureSingleAsync(
                StoredProcedures.Auth.sp_get_active_password_reset_token,
                parameters,
                reader => SqlDataReaderMapper.MapToDto<PasswordResetToken>(reader));
        }

        public async Task MarkAsUsedAsync(string token)
        {
            var parameters = new[]
            {
                CreateParameter("@Token", token, SqlDbType.VarChar)
            };

            await ExecuteStoredProcedureNonQueryAsync(StoredProcedures.Auth.sp_mark_password_reset_used, parameters);
        }

        public async Task UpdateUserPasswordAsync(int userId, string hashedPassword)
        {
            // Llama al SP que actualiza contraseña (recomendado)
            var parameters = new[]
            {
                CreateParameter("@UserId", userId, SqlDbType.Int),
                CreateParameter("@NewPassword", hashedPassword, SqlDbType.NVarChar)
            };

            await ExecuteStoredProcedureNonQueryAsync(StoredProcedures.Auth.sp_update_user_password, parameters);
        }
    }
}