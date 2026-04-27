namespace BusinessCentral.Application.DTOs.Manufacturing;

public sealed class RecipeItemUpsertDTO
{
    public int InputProductId { get; set; }
    public long? InputVariantId { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public string? Notes { get; set; }
}

