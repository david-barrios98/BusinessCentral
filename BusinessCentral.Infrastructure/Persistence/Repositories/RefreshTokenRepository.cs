using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Domain.Entities.Audit;
using BusinessCentral.Infrastructure.Constants;
using BusinessCentral.Infrastructure.Extensions;
using BusinessCentral.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : SqlConfigServer, IRefreshTokenRepository
    {
        public RefreshTokenRepository(IConfiguration configuration) : base(configuration) { }

        public async Task AddAsync(RefreshToken token)
        {
            var parameters = new[]
            {
                CreateParameter("@UserSessionId", token.UserSessionId, SqlDbType.BigInt),
                CreateParameter("@Token", token.Token, SqlDbType.NVarChar, 500),
                CreateParameter("@ExpiresAt", TimeZoneHelper.ConvertToColombiaTime(DateTime.UtcNow.AddMinutes(30)), SqlDbType.DateTime),
                CreateParameter("@CreatedAt", TimeZoneHelper.GetColombiaTimeNow(), SqlDbType.DateTime),
                CreateParameter("@JwtId", token.JwtId ?? (object)DBNull.Value, SqlDbType.VarChar),
                CreateParameter("@AccessTokenExpiresAt", token.AccessTokenExpiresAt ?? (object)DBNull.Value, SqlDbType.DateTime)
            };

            // Mapeo directo a long
            token.Id = await ExecuteStoredProcedureSingleAsync(
                StoredProcedures.Auth.sp_insert_refresh_token,
                parameters,
                reader => Convert.ToInt64(reader.GetValue(0)));
        }

        public async Task<LoginResponseDTO?> GetActiveByTokenAsync(string token)
        {
            var parameters = new[]
            {
                CreateParameter("@Token", token, SqlDbType.NVarChar, 500),
                CreateParameter("@CurrentTime", TimeZoneHelper.GetColombiaTimeNow(), SqlDbType.DateTime)
            };

            return await ExecuteStoredProcedureSingleAsync(
                StoredProcedures.Auth.sp_get_active_refresh_token,
                parameters,
                reader => SqlDataReaderMapper.MapToDto<LoginResponseDTO>(reader));
        }

        public async Task RevokeAsync(RefreshToken token, string? replacedByToken = null)
        {
            var parameters = new[]
            {
                CreateParameter("@Token", token.Token, SqlDbType.NVarChar, 500),
                CreateParameter("@RevokedAt", TimeZoneHelper.GetColombiaTimeNow(), SqlDbType.DateTime),
                CreateParameter("@ReplacedByToken", replacedByToken ?? (object)DBNull.Value, SqlDbType.NVarChar)
            };

            await ExecuteStoredProcedureNonQueryAsync(StoredProcedures.Auth.sp_revoke_refresh_token, parameters);
        }

        public async Task RevokeAllByUserAsync(long userSessionId, string? replacedByToken = null)
        {
            var now = TimeZoneHelper.GetColombiaTimeNow();
            var parameters = new[]
            {
                CreateParameter("@UserSessionId", userSessionId, SqlDbType.BigInt),
                CreateParameter("@RevokedAt", now, SqlDbType.DateTime),
                CreateParameter("@CurrentTime", now, SqlDbType.DateTime),
                CreateParameter("@ReplacedByToken", replacedByToken ?? (object)DBNull.Value, SqlDbType.VarChar)
            };

            await ExecuteStoredProcedureNonQueryAsync(StoredProcedures.Auth.sp_revoke_all_tokens_by_user, parameters);
        }

        public async Task RevokeAllByCompanyAsync(int companyId, string? replacedByToken = null)
        {
            var now = TimeZoneHelper.GetColombiaTimeNow();
            var parameters = new[]
            {
                CreateParameter("@CompanyId", companyId, SqlDbType.Int),
                CreateParameter("@RevokedAt", now, SqlDbType.DateTime),
                CreateParameter("@CurrentTime", now, SqlDbType.DateTime),
                CreateParameter("@ReplacedByToken", replacedByToken ?? (object)DBNull.Value, SqlDbType.VarChar)
            };

            await ExecuteStoredProcedureNonQueryAsync(StoredProcedures.Auth.sp_revoke_all_tokens_by_company, parameters);
        }
    }
}