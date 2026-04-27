using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class ProductVariantRepository : SqlConfigServer, IProductVariantRepository
{
    public ProductVariantRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<long> UpsertAsync(int companyId, long? id, int productId, string sku, string? barcode, string? variantName, decimal? priceOverride, decimal? costOverride, bool active)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@id", (object?)id ?? DBNull.Value, SqlDbType.BigInt),
            CreateParameter("@product_id", productId, SqlDbType.Int),
            CreateParameter("@sku", sku, SqlDbType.NVarChar, 80),
            CreateParameter("@barcode", (object?)barcode ?? DBNull.Value, SqlDbType.NVarChar, 50),
            CreateParameter("@variant_name", (object?)variantName ?? DBNull.Value, SqlDbType.NVarChar, 200),
            CreateParameter("@price_override", (object?)priceOverride ?? DBNull.Value, SqlDbType.Decimal),
            CreateParameter("@cost_override", (object?)costOverride ?? DBNull.Value, SqlDbType.Decimal),
            CreateParameter("@active", active, SqlDbType.Bit)
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Commerce.sp_upsert_product_variant,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return insertedId;
    }

    public async Task<List<ProductVariantListItemDTO>> ListAsync(int companyId, int? productId = null, bool onlyActive = true, string? q = null)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@product_id", (object?)productId ?? DBNull.Value, SqlDbType.Int),
            CreateParameter("@only_active", onlyActive, SqlDbType.Bit),
            CreateParameter("@q", (object?)q ?? DBNull.Value, SqlDbType.NVarChar, 100)
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Commerce.sp_list_product_variants,
            parameters,
            r => new ProductVariantListItemDTO
            {
                Id = Convert.ToInt64(r["Id"]),
                CompanyId = Convert.ToInt32(r["CompanyId"]),
                ProductId = Convert.ToInt32(r["ProductId"]),
                ProductSku = r["ProductSku"]?.ToString() ?? string.Empty,
                ProductName = r["ProductName"]?.ToString() ?? string.Empty,
                Sku = r["Sku"]?.ToString() ?? string.Empty,
                Barcode = r["Barcode"] == DBNull.Value ? null : r["Barcode"]?.ToString(),
                VariantName = r["VariantName"] == DBNull.Value ? null : r["VariantName"]?.ToString(),
                PriceOverride = r["PriceOverride"] == DBNull.Value ? null : Convert.ToDecimal(r["PriceOverride"]),
                CostOverride = r["CostOverride"] == DBNull.Value ? null : Convert.ToDecimal(r["CostOverride"]),
                Active = Convert.ToBoolean(r["Active"])
            });
    }
}

