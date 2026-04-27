namespace BusinessCentral.Application.DTOs.Manufacturing;

public sealed class RecipeCostLineDTO
{
    public int InputProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal UnitCost { get; set; }
    public decimal LineCost { get; set; }
}

