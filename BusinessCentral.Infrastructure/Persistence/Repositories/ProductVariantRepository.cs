using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

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

    public async Task<PagedResult<ProductVariantListItemDTO>> ListAsync(int companyId, int? productId = null, bool onlyActive = true, int page = 1, int pageSize = 50, string? q = null)
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(StoredProcedures.Commerce.sp_list_product_variants, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(CreateParameter("@company_id", companyId, SqlDbType.Int));
        command.Parameters.Add(CreateParameter("@product_id", (object?)productId ?? DBNull.Value, SqlDbType.Int));
        command.Parameters.Add(CreateParameter("@only_active", onlyActive, SqlDbType.Bit));
        command.Parameters.Add(CreateParameter("@q", (object?)q ?? DBNull.Value, SqlDbType.NVarChar, 100));
        command.Parameters.Add(CreateParameter("@page", page, SqlDbType.Int));
        command.Parameters.Add(CreateParameter("@page_size", pageSize, SqlDbType.Int));

        var items = new List<ProductVariantListItemDTO>();
        long total = 0;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(new ProductVariantListItemDTO
            {
                Id = Convert.ToInt64(reader["Id"]),
                CompanyId = Convert.ToInt32(reader["CompanyId"]),
                ProductId = Convert.ToInt32(reader["ProductId"]),
                ProductSku = reader["ProductSku"]?.ToString() ?? string.Empty,
                ProductName = reader["ProductName"]?.ToString() ?? string.Empty,
                Sku = reader["Sku"]?.ToString() ?? string.Empty,
                Barcode = reader["Barcode"] == DBNull.Value ? null : reader["Barcode"]?.ToString(),
                VariantName = reader["VariantName"] == DBNull.Value ? null : reader["VariantName"]?.ToString(),
                PriceOverride = reader["PriceOverride"] == DBNull.Value ? null : Convert.ToDecimal(reader["PriceOverride"]),
                CostOverride = reader["CostOverride"] == DBNull.Value ? null : Convert.ToDecimal(reader["CostOverride"]),
                Active = Convert.ToBoolean(reader["Active"])
            });
        }

        if (await reader.NextResultAsync() && await reader.ReadAsync())
            total = Convert.ToInt64(reader["Total"]);

        return new PagedResult<ProductVariantListItemDTO>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }
}

