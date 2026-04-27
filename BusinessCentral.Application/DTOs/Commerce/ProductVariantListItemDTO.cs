namespace BusinessCentral.Application.DTOs.Commerce;

public sealed class ProductVariantListItemDTO
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public int ProductId { get; set; }
    public string ProductSku { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string? VariantName { get; set; }
    public decimal? PriceOverride { get; set; }
    public decimal? CostOverride { get; set; }
    public bool Active { get; set; }
}

