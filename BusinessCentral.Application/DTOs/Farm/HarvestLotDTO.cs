namespace BusinessCentral.Application.DTOs.Farm;

public sealed class HarvestLotDTO
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public int? ZoneId { get; set; }
    public string? ZoneName { get; set; }
    public DateTime HarvestDate { get; set; }
    public string ProductForm { get; set; } = string.Empty;
    public decimal QuantityKg { get; set; }
    public string? Notes { get; set; }
}

