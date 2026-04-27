namespace BusinessCentral.Application.DTOs.Commerce;

public sealed class PurchaseReceiptDTO
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public long? SupplierId { get; set; }
    public DateTime ReceiptDate { get; set; }
    public string? SupplierInvoiceNumber { get; set; }
    public string Status { get; set; } = "draft";
    public decimal Total { get; set; }
    public long? DefaultToLocationId { get; set; }
}

