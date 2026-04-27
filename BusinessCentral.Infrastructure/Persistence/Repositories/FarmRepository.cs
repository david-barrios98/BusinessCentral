using BusinessCentral.Application.DTOs.Farm;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class FarmRepository : SqlConfigServer, IFarmRepository
{
    public FarmRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<bool> UpsertZoneAsync(int companyId, string code, string name, bool active)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@code", code, SqlDbType.NVarChar, 50),
            CreateParameter("@name", name, SqlDbType.NVarChar, 150),
            CreateParameter("@active", active, SqlDbType.Bit),
        };

        var success = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Farm.sp_upsert_zone,
            parameters,
            r => Convert.ToBoolean(r["Success"])
        );

        return success == true;
    }

    public async Task<List<FarmZoneDTO>> ListZonesAsync(int companyId, bool onlyActive)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@only_active", onlyActive, SqlDbType.Bit),
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Farm.sp_list_zones,
            parameters,
            r => new FarmZoneDTO
            {
                Id = Convert.ToInt32(r["Id"]),
                CompanyId = Convert.ToInt32(r["CompanyId"]),
                Code = r["Code"]?.ToString() ?? string.Empty,
                Name = r["Name"]?.ToString() ?? string.Empty,
                Active = Convert.ToBoolean(r["Active"]),
            }
        );
    }

    public async Task<long> CreateHarvestLotAsync(int companyId, HarvestLotDTO dto)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@zone_id", (object?)dto.ZoneId ?? DBNull.Value, SqlDbType.Int),
            CreateParameter("@harvest_date", dto.HarvestDate.Date, SqlDbType.Date),
            CreateParameter("@product_form", dto.ProductForm, SqlDbType.NVarChar, 50),
            CreateParameter("@quantity_kg", dto.QuantityKg, SqlDbType.Decimal),
            CreateParameter("@notes", (object?)dto.Notes ?? DBNull.Value, SqlDbType.NVarChar, 500),
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Farm.sp_create_harvest_lot,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return insertedId;
    }

    public async Task<List<HarvestLotDTO>> ListHarvestLotsAsync(int companyId, DateTime? fromDate, DateTime? toDate, int? zoneId)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@from_date", (object?)fromDate?.Date ?? DBNull.Value, SqlDbType.Date),
            CreateParameter("@to_date", (object?)toDate?.Date ?? DBNull.Value, SqlDbType.Date),
            CreateParameter("@zone_id", (object?)zoneId ?? DBNull.Value, SqlDbType.Int),
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Farm.sp_list_harvest_lots,
            parameters,
            r => new HarvestLotDTO
            {
                Id = Convert.ToInt64(r["Id"]),
                CompanyId = Convert.ToInt32(r["CompanyId"]),
                ZoneId = r["ZoneId"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["ZoneId"]),
                ZoneName = r["ZoneName"] == DBNull.Value ? null : r["ZoneName"]?.ToString(),
                HarvestDate = Convert.ToDateTime(r["HarvestDate"]),
                ProductForm = r["ProductForm"]?.ToString() ?? string.Empty,
                QuantityKg = Convert.ToDecimal(r["QuantityKg"]),
                Notes = r["Notes"] == DBNull.Value ? null : r["Notes"]?.ToString(),
            }
        );
    }

    public async Task<long> CreateProcessStepAsync(int companyId, CoffeeProcessStepDTO dto)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@harvest_lot_id", dto.HarvestLotId, SqlDbType.BigInt),
            CreateParameter("@step_date", dto.StepDate.Date, SqlDbType.Date),
            CreateParameter("@step_type", dto.StepType, SqlDbType.NVarChar, 50),
            CreateParameter("@input_kg", (object?)dto.InputKg ?? DBNull.Value, SqlDbType.Decimal),
            CreateParameter("@output_kg", (object?)dto.OutputKg ?? DBNull.Value, SqlDbType.Decimal),
            CreateParameter("@notes", (object?)dto.Notes ?? DBNull.Value, SqlDbType.NVarChar, 500),
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Farm.sp_create_process_step,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return insertedId;
    }

    public async Task<List<CoffeeProcessStepDTO>> ListProcessStepsAsync(int companyId, long harvestLotId)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@harvest_lot_id", harvestLotId, SqlDbType.BigInt),
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Farm.sp_list_process_steps,
            parameters,
            r => new CoffeeProcessStepDTO
            {
                Id = Convert.ToInt64(r["Id"]),
                CompanyId = Convert.ToInt32(r["CompanyId"]),
                HarvestLotId = Convert.ToInt64(r["HarvestLotId"]),
                StepDate = Convert.ToDateTime(r["StepDate"]),
                StepType = r["StepType"]?.ToString() ?? string.Empty,
                InputKg = r["InputKg"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(r["InputKg"]),
                OutputKg = r["OutputKg"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(r["OutputKg"]),
                Notes = r["Notes"] == DBNull.Value ? null : r["Notes"]?.ToString(),
            }
        );
    }
}

