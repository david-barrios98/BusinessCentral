using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Adapters
{
    public class ApuRepository : SqlConfigServer, IApuRepository
    {
        public ApuRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<int?> InsertApuItemAsync(int projectId, string? code, string? description, string unit, decimal unitPrice, decimal yield)
        {
            var parameters = new[]
            {
                CreateParameter("@ProjectId", projectId, SqlDbType.Int),
                CreateParameter("@Code", code ?? string.Empty, SqlDbType.VarChar),
                CreateParameter("@Description", description ?? string.Empty, SqlDbType.VarChar),
                CreateParameter("@Unit", unit, SqlDbType.VarChar),
                CreateParameter("@UnitPrice", unitPrice, SqlDbType.Decimal),
                CreateParameter("@Yield", yield, SqlDbType.Decimal)
            };

            var id = await ExecuteStoredProcedureSingleAsync(
                "[construction].[sp_insert_apu_item]",
                parameters,
                reader => Convert.ToInt32(reader.GetValue(0)));

            return id;
        }

        public async Task<List<ApuItemDto>> ListApuItemsAsync(int projectId)
        {
            // Si aún no existe SP de listado, se puede crear; aquí asumo sp_list_apu_items
            var parameters = new[]
            {
                CreateParameter("@ProjectId", projectId, SqlDbType.Int)
            };

            return await ExecuteStoredProcedureAsync(
                "[construction].[sp_list_apu_items]",
                parameters,
                reader => SqlDataReaderMapper.MapToDto<ApuItemDto>(reader));
        }
    }
}