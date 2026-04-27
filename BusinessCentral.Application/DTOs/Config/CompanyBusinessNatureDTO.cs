namespace BusinessCentral.Application.DTOs.Config;

public sealed class CompanyBusinessNatureDTO
{
    public int CompanyId { get; set; }
    public int BusinessNatureId { get; set; }
    public string NatureCode { get; set; } = string.Empty;
    public string NatureName { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public DateTime CreatedAt { get; set; }
}

