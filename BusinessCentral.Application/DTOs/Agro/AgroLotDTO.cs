namespace BusinessCentral.Application.DTOs.Agro;

public sealed class AgroLotDTO
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public string Kind { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Name { get; set; }
    public DateTime StartDate { get; set; }
    public int InitialUnits { get; set; }
    public int CurrentUnits { get; set; }
    public decimal? InitialAvgWeightKg { get; set; }
    public string Status { get; set; } = "open";
    public string? Notes { get; set; }
}

