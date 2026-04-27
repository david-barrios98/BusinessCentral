namespace BusinessCentral.Application.DTOs.Commerce;

public sealed class InventoryByLocationRowDTO
{
    public int ProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public long? LocationId { get; set; }
    public string? LocationCode { get; set; }
    public string? LocationName { get; set; }
    public decimal QuantityOnHand { get; set; }
}

