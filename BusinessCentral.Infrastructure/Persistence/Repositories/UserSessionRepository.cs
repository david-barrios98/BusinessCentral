using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Domain.Entities.Audit;
using BusinessCentral.Infrastructure.Constants;
using BusinessCentral.Infrastructure.Extensions;
using BusinessCentral.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories
{
    public class UserSessionRepository : SqlConfigServer, IUserSessionRepository
    {
        public UserSessionRepository(IConfiguration configuration) : base(configuration) { }

        public async Task AddAsync(UserSession session)
        {
            var parameters = new[]
            {
                CreateParameter("@UserId", session.UserId, SqlDbType.Int),
                CreateParameter("@LoginField", session.LoginField ?? (object)DBNull.Value, SqlDbType.VarChar),
                CreateParameter("@CompanyId", session.CompanyId, SqlDbType.Int),
                CreateParameter("@Platform", session.Platform, SqlDbType.VarChar),
                CreateParameter("@DeviceFingerprint", session.DeviceFingerprint ?? (object)DBNull.Value, SqlDbType.VarChar),
                CreateParameter("@IpAddress", session.IpAddress ?? (object)DBNull.Value, SqlDbType.VarChar),
                CreateParameter("@UserAgent", session.UserAgent ?? (object)DBNull.Value, SqlDbType.VarChar),
                CreateParameter("@LoginAt", TimeZoneHelper.GetColombiaTimeNow(), SqlDbType.DateTime),
                CreateParameter("@IsSuccess", session.IsSuccess, SqlDbType.Bit),
                CreateParameter("@FailureReason", session.FailureReason ?? (object)DBNull.Value, SqlDbType.VarChar)
            };

            session.Id = await ExecuteStoredProcedureSingleAsync(
                StoredProcedures.Audit.sp_insert_user_session,
                parameters,
                reader => Convert.ToInt64(reader.GetValue(0)));
        }

        public async Task UpdateAsync(UserSession session)
        {
            var parameters = new[]
            {
                CreateParameter("@Id", session.Id, SqlDbType.BigInt),
                CreateParameter("@LogoutAt", session.LogoutAt ?? (object)DBNull.Value, SqlDbType.DateTime),
                CreateParameter("@IsSuccess", session.IsSuccess, SqlDbType.Bit),
                CreateParameter("@FailureReason", session.FailureReason ?? (object)DBNull.Value, SqlDbType.VarChar)
            };

            await ExecuteStoredProcedureNonQueryAsync(StoredProcedures.Audit.sp_update_user_session, parameters);
        }

        public async Task<UserSession?> GetByIdAsync(long id)
        {
            var parameters = new[] { CreateParameter("@Id", id, SqlDbType.BigInt) };

            return await ExecuteStoredProcedureSingleAsync(
                StoredProcedures.Audit.sp_get_user_session_by_id,
                parameters,
                reader => SqlDataReaderMapper.MapToDto<UserSession>(reader));
        }

        public async Task CloseSessionsByUserAsync(int userId)
        {
            var parameters = new[]
            {
                CreateParameter("@UserId", userId, SqlDbType.Int),
                CreateParameter("@LogoutAt", TimeZoneHelper.GetColombiaTimeNow(), SqlDbType.DateTime)
            };

            await ExecuteStoredProcedureNonQueryAsync(StoredProcedures.Audit.sp_close_all_user_sessions, parameters);
        }

        public async Task CloseSessionsByCompanyAsync(int companyId)
        {
            var parameters = new[]
            {
                CreateParameter("@CompanyId", companyId, SqlDbType.Int),
                CreateParameter("@LogoutAt", TimeZoneHelper.GetColombiaTimeNow(), SqlDbType.DateTime)
            };

            await ExecuteStoredProcedureNonQueryAsync(StoredProcedures.Audit.sp_close_company_sessions, parameters);
        }
    }
}