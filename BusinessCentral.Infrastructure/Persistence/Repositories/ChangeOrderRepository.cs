using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Adapters
{
    public class ChangeOrderRepository : SqlConfigServer, IChangeOrderRepository
    {
        public ChangeOrderRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<int?> CreateChangeOrderAsync(int projectId, string description, decimal amount, int? requestedByUserId)
        {
            var parameters = new[]
            {
                CreateParameter("@ProjectId", projectId, SqlDbType.Int),
                CreateParameter("@Description", description ?? string.Empty, SqlDbType.VarChar),
                CreateParameter("@Amount", amount, SqlDbType.Decimal),
                CreateParameter("@RequestedByUserId", requestedByUserId ?? (object)DBNull.Value, SqlDbType.Int)
            };

            var id = await ExecuteStoredProcedureSingleAsync(
                "[construction].[sp_create_change_order]",
                parameters,
                reader => Convert.ToInt32(reader.GetValue(0)));

            return id;
        }

        public async Task<List<ChangeOrderDto>> ListChangeOrdersAsync(int projectId)
        {
            var parameters = new[]
            {
                CreateParameter("@ProjectId", projectId, SqlDbType.Int)
            };

            return await ExecuteStoredProcedureAsync(
                "[construction].[sp_list_change_orders]",
                parameters,
                reader => SqlDataReaderMapper.MapToDto<ChangeOrderDto>(reader));
        }
    }
}