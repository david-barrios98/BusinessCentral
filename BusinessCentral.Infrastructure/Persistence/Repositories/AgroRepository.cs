using BusinessCentral.Application.DTOs.Agro;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class AgroRepository : SqlConfigServer, IAgroRepository
{
    public AgroRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<long> CreateLotAsync(int companyId, string kind, string code, string? name, DateTime startDate, int initialUnits, decimal? initialAvgWeightKg, string? notes)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@kind", kind, SqlDbType.NVarChar, 20),
            CreateParameter("@code", code, SqlDbType.NVarChar, 50),
            CreateParameter("@name", (object?)name ?? DBNull.Value, SqlDbType.NVarChar, 200),
            CreateParameter("@start_date", startDate, SqlDbType.DateTime2),
            CreateParameter("@initial_units", initialUnits, SqlDbType.Int),
            CreateParameter("@initial_avg_weight_kg", (object?)initialAvgWeightKg ?? DBNull.Value, SqlDbType.Decimal),
            CreateParameter("@notes", (object?)notes ?? DBNull.Value, SqlDbType.NVarChar, 500)
        };

        var id = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Agro.sp_create_lot,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return id;
    }

    public async Task<PagedResult<AgroLotDTO>> ListLotsAsync(int companyId, string? kind = null, bool onlyOpen = false, int page = 1, int pageSize = 50)
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(StoredProcedures.Agro.sp_list_lots, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(CreateParameter("@company_id", companyId, SqlDbType.Int));
        command.Parameters.Add(CreateParameter("@kind", (object?)kind ?? DBNull.Value, SqlDbType.NVarChar, 20));
        command.Parameters.Add(CreateParameter("@only_open", onlyOpen, SqlDbType.Bit));
        command.Parameters.Add(CreateParameter("@page", page, SqlDbType.Int));
        command.Parameters.Add(CreateParameter("@page_size", pageSize, SqlDbType.Int));

        var items = new List<AgroLotDTO>();
        long total = 0;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(new AgroLotDTO
            {
                Id = Convert.ToInt64(reader["Id"]),
                CompanyId = Convert.ToInt32(reader["CompanyId"]),
                Kind = reader["Kind"]?.ToString() ?? string.Empty,
                Code = reader["Code"]?.ToString() ?? string.Empty,
                Name = reader["Name"] == DBNull.Value ? null : reader["Name"]?.ToString(),
                StartDate = Convert.ToDateTime(reader["StartDate"]),
                InitialUnits = Convert.ToInt32(reader["InitialUnits"]),
                CurrentUnits = Convert.ToInt32(reader["CurrentUnits"]),
                InitialAvgWeightKg = reader["InitialAvgWeightKg"] == DBNull.Value ? null : Convert.ToDecimal(reader["InitialAvgWeightKg"]),
                Status = reader["Status"]?.ToString() ?? "open",
                Notes = reader["Notes"] == DBNull.Value ? null : reader["Notes"]?.ToString()
            });
        }

        if (await reader.NextResultAsync() && await reader.ReadAsync())
            total = Convert.ToInt64(reader["Total"]);

        return new PagedResult<AgroLotDTO>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }

    public async Task<long> CreateFeedLogAsync(int companyId, long lotId, DateTime feedDate, int feedProductId, long? feedVariantId, decimal quantity, long? fromLocationId, decimal? unitCost, string? notes)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@lot_id", lotId, SqlDbType.BigInt),
            CreateParameter("@feed_date", feedDate, SqlDbType.DateTime2),
            CreateParameter("@feed_product_id", feedProductId, SqlDbType.Int),
            CreateParameter("@feed_variant_id", (object?)feedVariantId ?? DBNull.Value, SqlDbType.BigInt),
            CreateParameter("@quantity", quantity, SqlDbType.Decimal),
            CreateParameter("@from_location_id", (object?)fromLocationId ?? DBNull.Value, SqlDbType.BigInt),
            CreateParameter("@unit_cost", (object?)unitCost ?? DBNull.Value, SqlDbType.Decimal),
            CreateParameter("@notes", (object?)notes ?? DBNull.Value, SqlDbType.NVarChar, 500)
        };

        var id = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Agro.sp_create_feed_log,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return id;
    }

    public async Task<long> CreateMortalityLogAsync(int companyId, long lotId, DateTime mortalityDate, int units, decimal? avgWeightKg, string? notes)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@lot_id", lotId, SqlDbType.BigInt),
            CreateParameter("@mortality_date", mortalityDate, SqlDbType.DateTime2),
            CreateParameter("@units", units, SqlDbType.Int),
            CreateParameter("@avg_weight_kg", (object?)avgWeightKg ?? DBNull.Value, SqlDbType.Decimal),
            CreateParameter("@notes", (object?)notes ?? DBNull.Value, SqlDbType.NVarChar, 500)
        };

        var id = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Agro.sp_create_mortality_log,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return id;
    }

    public async Task<long> CreateHarvestAsync(int companyId, long lotId, DateTime harvestDate, int outputProductId, long? outputVariantId, int units, decimal? totalWeightKg, long? toLocationId, string? notes)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@lot_id", lotId, SqlDbType.BigInt),
            CreateParameter("@harvest_date", harvestDate, SqlDbType.DateTime2),
            CreateParameter("@output_product_id", outputProductId, SqlDbType.Int),
            CreateParameter("@output_variant_id", (object?)outputVariantId ?? DBNull.Value, SqlDbType.BigInt),
            CreateParameter("@units", units, SqlDbType.Int),
            CreateParameter("@total_weight_kg", (object?)totalWeightKg ?? DBNull.Value, SqlDbType.Decimal),
            CreateParameter("@to_location_id", (object?)toLocationId ?? DBNull.Value, SqlDbType.BigInt),
            CreateParameter("@notes", (object?)notes ?? DBNull.Value, SqlDbType.NVarChar, 500)
        };

        var id = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Agro.sp_create_harvest,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return id;
    }

    public async Task<AgroLotKpisDTO> GetLotKpisAsync(int companyId, long lotId)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@lot_id", lotId, SqlDbType.BigInt)
        };

        var data = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Agro.sp_report_lot_kpis,
            parameters,
            r => new AgroLotKpisDTO
            {
                LotId = Convert.ToInt64(r["LotId"]),
                InitialUnits = Convert.ToInt32(r["InitialUnits"]),
                CurrentUnits = Convert.ToInt32(r["CurrentUnits"]),
                MortalityUnits = Convert.ToInt32(r["MortalityUnits"]),
                MortalityRate = Convert.ToDecimal(r["MortalityRate"]),
                FeedQty = Convert.ToDecimal(r["FeedQty"]),
                FeedCost = Convert.ToDecimal(r["FeedCost"])
            });

        return data ?? new AgroLotKpisDTO { LotId = lotId };
    }
}

