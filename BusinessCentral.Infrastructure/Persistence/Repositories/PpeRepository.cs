using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Adapters
{
    public class PpeRepository : SqlConfigServer, IPpeRepository
    {
        public PpeRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<int?> InsertPpeRecordAsync(int? projectId, int userId, string item, string? notes)
        {
            var parameters = new[]
            {
                CreateParameter("@ProjectId", projectId ?? (object)DBNull.Value, SqlDbType.Int),
                CreateParameter("@UserId", userId, SqlDbType.Int),
                CreateParameter("@Item", item ?? string.Empty, SqlDbType.VarChar),
                CreateParameter("@Notes", notes ?? string.Empty, SqlDbType.VarChar)
            };

            var id = await ExecuteStoredProcedureSingleAsync(
                "[construction].[sp_insert_ppe_record]",
                parameters,
                reader => Convert.ToInt32(reader.GetValue(0)));

            return id;
        }

        public async Task<List<PpeDto>> ListPpeAsync(int projectId)
        {
            var parameters = new[]
            {
                CreateParameter("@ProjectId", projectId, SqlDbType.Int)
            };

            return await ExecuteStoredProcedureAsync(
                "[construction].[sp_list_ppe]",
                parameters,
                reader => SqlDataReaderMapper.MapToDto<PpeDto>(reader));
        }
    }
}