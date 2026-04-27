using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class InventoryLocationReportRepository : SqlConfigServer, IInventoryLocationReportRepository
{
    public InventoryLocationReportRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<List<InventoryByLocationRowDTO>> GetInventoryByLocationAsync(int companyId, DateTime asOfUtc, long? locationId = null)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@as_of", asOfUtc, SqlDbType.DateTime2),
            CreateParameter("@location_id", (object?)locationId ?? DBNull.Value, SqlDbType.BigInt)
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Commerce.sp_report_inventory_by_location,
            parameters,
            reader => new InventoryByLocationRowDTO
            {
                ProductId = Convert.ToInt32(reader["ProductId"]),
                Sku = reader["Sku"]?.ToString() ?? string.Empty,
                ProductName = reader["ProductName"]?.ToString() ?? string.Empty,
                LocationId = reader["LocationId"] == DBNull.Value ? null : Convert.ToInt64(reader["LocationId"]),
                LocationCode = reader["LocationCode"] == DBNull.Value ? null : reader["LocationCode"]?.ToString(),
                LocationName = reader["LocationName"] == DBNull.Value ? null : reader["LocationName"]?.ToString(),
                QuantityOnHand = Convert.ToDecimal(reader["QuantityOnHand"])
            });
    }
}

