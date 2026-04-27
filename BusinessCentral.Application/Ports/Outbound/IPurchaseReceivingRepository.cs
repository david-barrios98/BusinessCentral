using BusinessCentral.Application.DTOs.Commerce;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IPurchaseReceivingRepository
{
    Task<long> CreateReceiptAsync(int companyId, long? supplierId, DateTime receiptDate, string? supplierInvoiceNumber, long? defaultToLocationId, int? createdByUserId);
    Task<long> AddReceiptLineAsync(int companyId, long receiptId, int productId, long? variantId, decimal quantity, decimal unitCost, long? toLocationId, string? notes);
    Task<bool> PostReceiptAsync(int companyId, long receiptId);
}

