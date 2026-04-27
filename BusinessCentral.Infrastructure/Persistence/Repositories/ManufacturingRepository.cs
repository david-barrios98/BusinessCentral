using BusinessCentral.Application.DTOs.Manufacturing;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text.Json;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class ManufacturingRepository : SqlConfigServer, IManufacturingRepository
{
    public ManufacturingRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<long> UpsertRecipeAsync(int companyId, long? id, int outputProductId, long? outputVariantId, decimal outputQuantity, string? notes, bool active)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@id", (object?)id ?? DBNull.Value, SqlDbType.BigInt),
            CreateParameter("@output_product_id", outputProductId, SqlDbType.Int),
            CreateParameter("@output_variant_id", (object?)outputVariantId ?? DBNull.Value, SqlDbType.BigInt),
            CreateParameter("@output_quantity", outputQuantity, SqlDbType.Decimal),
            CreateParameter("@notes", (object?)notes ?? DBNull.Value, SqlDbType.NVarChar, 500),
            CreateParameter("@active", active, SqlDbType.Bit),
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Manufacturing.sp_upsert_recipe,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return insertedId;
    }

    public async Task<bool> SetRecipeItemsAsync(int companyId, long recipeId, List<RecipeItemUpsertDTO> items)
    {
        var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@recipe_id", recipeId, SqlDbType.BigInt),
            CreateParameter("@items_json", json, SqlDbType.NVarChar)
        };

        var ok = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Manufacturing.sp_set_recipe_items,
            parameters,
            r => Convert.ToBoolean(r["Success"])
        );

        return ok == true;
    }

    public async Task<long> CreateBatchAsync(int companyId, long? recipeId, int outputProductId, long? outputVariantId, decimal quantityProduced, long? toLocationId, DateTime batchDate, string? notes)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@recipe_id", (object?)recipeId ?? DBNull.Value, SqlDbType.BigInt),
            CreateParameter("@output_product_id", outputProductId, SqlDbType.Int),
            CreateParameter("@output_variant_id", (object?)outputVariantId ?? DBNull.Value, SqlDbType.BigInt),
            CreateParameter("@quantity_produced", quantityProduced, SqlDbType.Decimal),
            CreateParameter("@to_location_id", (object?)toLocationId ?? DBNull.Value, SqlDbType.BigInt),
            CreateParameter("@batch_date", batchDate, SqlDbType.DateTime2),
            CreateParameter("@notes", (object?)notes ?? DBNull.Value, SqlDbType.NVarChar, 500)
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Manufacturing.sp_create_production_batch,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return insertedId;
    }

    public async Task<bool> PostBatchAsync(int companyId, long batchId)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@batch_id", batchId, SqlDbType.BigInt)
        };

        var ok = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Manufacturing.sp_post_production_batch,
            parameters,
            r => Convert.ToBoolean(r["Success"])
        );

        return ok == true;
    }

    public async Task<RecipeCostReportDTO> GetRecipeCostAsync(int companyId, long recipeId, decimal quantity)
    {
        // StoredProcedure returns 2 resultsets; reuse low-level multiple reader approach via SqlClient not present here.
        // We will use ExecuteStoredProcedureAsync twice by calling once for lines and once for total, via same SP and reading only first/second is not supported.
        // Solution: call SP once for lines and compute total locally (matches total resultset).
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@recipe_id", recipeId, SqlDbType.BigInt),
            CreateParameter("@quantity", quantity, SqlDbType.Decimal)
        };

        var lines = await ExecuteStoredProcedureAsync(
            StoredProcedures.Manufacturing.sp_report_recipe_cost,
            parameters,
            r => new RecipeCostLineDTO
            {
                InputProductId = Convert.ToInt32(r["InputProductId"]),
                Sku = r["Sku"]?.ToString() ?? string.Empty,
                ProductName = r["ProductName"]?.ToString() ?? string.Empty,
                Qty = Convert.ToDecimal(r["Qty"]),
                UnitCost = Convert.ToDecimal(r["UnitCost"]),
                LineCost = Convert.ToDecimal(r["LineCost"])
            }
        );

        return new RecipeCostReportDTO
        {
            Lines = lines,
            TotalCost = lines.Sum(x => x.LineCost)
        };
    }
}

