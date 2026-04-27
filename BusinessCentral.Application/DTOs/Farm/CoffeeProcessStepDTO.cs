namespace BusinessCentral.Application.DTOs.Farm;

public sealed class CoffeeProcessStepDTO
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public long HarvestLotId { get; set; }
    public DateTime StepDate { get; set; }
    public string StepType { get; set; } = string.Empty;
    public decimal? InputKg { get; set; }
    public decimal? OutputKg { get; set; }
    public string? Notes { get; set; }
}

