using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class PurchaseReceivingRepository : SqlConfigServer, IPurchaseReceivingRepository
{
    public PurchaseReceivingRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<long> CreateReceiptAsync(int companyId, long? supplierId, DateTime receiptDate, string? supplierInvoiceNumber, long? defaultToLocationId, int? createdByUserId)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@supplier_id", (object?)supplierId ?? DBNull.Value, SqlDbType.BigInt),
            CreateParameter("@receipt_date", receiptDate, SqlDbType.DateTime2),
            CreateParameter("@supplier_invoice_number", (object?)supplierInvoiceNumber ?? DBNull.Value, SqlDbType.NVarChar, 50),
            CreateParameter("@default_to_location_id", (object?)defaultToLocationId ?? DBNull.Value, SqlDbType.BigInt),
            CreateParameter("@created_by_user_id", (object?)createdByUserId ?? DBNull.Value, SqlDbType.Int)
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Commerce.sp_create_purchase_receipt,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return insertedId;
    }

    public async Task<long> AddReceiptLineAsync(int companyId, long receiptId, int productId, long? variantId, decimal quantity, decimal unitCost, long? toLocationId, string? notes)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@receipt_id", receiptId, SqlDbType.BigInt),
            CreateParameter("@product_id", productId, SqlDbType.Int),
            CreateParameter("@variant_id", (object?)variantId ?? DBNull.Value, SqlDbType.BigInt),
            CreateParameter("@quantity", quantity, SqlDbType.Decimal),
            CreateParameter("@unit_cost", unitCost, SqlDbType.Decimal),
            CreateParameter("@to_location_id", (object?)toLocationId ?? DBNull.Value, SqlDbType.BigInt),
            CreateParameter("@notes", (object?)notes ?? DBNull.Value, SqlDbType.NVarChar, 500)
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Commerce.sp_add_purchase_receipt_line,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return insertedId;
    }

    public async Task<bool> PostReceiptAsync(int companyId, long receiptId)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@receipt_id", receiptId, SqlDbType.BigInt)
        };

        var ok = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Commerce.sp_post_purchase_receipt,
            parameters,
            r => Convert.ToBoolean(r["Success"])
        );

        return ok == true;
    }
}

