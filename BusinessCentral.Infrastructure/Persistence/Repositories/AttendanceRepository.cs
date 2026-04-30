using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using BusinessCentral.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Adapters
{
    public class AttendanceRepository : SqlConfigServer, IAttendanceRepository
    {
        public AttendanceRepository(IConfiguration configuration) : base(configuration) { }

        public async Task InsertAttendanceAsync(int projectId, int userId, DateTime dateWorked, decimal hours)
        {
            var parameters = new[]
            {
                CreateParameter("@ProjectId", projectId, SqlDbType.Int),
                CreateParameter("@UserId", userId, SqlDbType.Int),
                CreateParameter("@DateWorked", dateWorked, SqlDbType.Date),
                CreateParameter("@Hours", hours, SqlDbType.Decimal)
            };

            await ExecuteStoredProcedureNonQueryAsync(StoredProcedures.Construction.sp_insert_attendance, parameters);
        }

        public async Task<List<AttendanceDto>> ListAttendanceAsync(int projectId, DateTime? from = null, DateTime? to = null)
        {
            var parameters = new[]
            {
                CreateParameter("@ProjectId", projectId, SqlDbType.Int),
                CreateParameter("@FromDate", from ?? (object)DBNull.Value, SqlDbType.DateTime2),
                CreateParameter("@ToDate", to ?? (object)DBNull.Value, SqlDbType.DateTime2)
            };

            return await ExecuteStoredProcedureAsync(
                 StoredProcedures.Construction.sp_list_attendance,
                parameters,
                reader => SqlDataReaderMapper.MapToDto<AttendanceDto>(reader));
        }
    }
}