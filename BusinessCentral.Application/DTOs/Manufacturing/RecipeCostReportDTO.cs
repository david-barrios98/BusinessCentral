namespace BusinessCentral.Application.DTOs.Manufacturing;

public sealed class RecipeCostReportDTO
{
    public List<RecipeCostLineDTO> Lines { get; set; } = new();
    public decimal TotalCost { get; set; }
}

