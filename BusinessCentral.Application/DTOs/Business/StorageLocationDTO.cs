namespace BusinessCentral.Application.DTOs.Business;

public sealed class StorageLocationDTO
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public int? FacilityId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "WAREHOUSE";
    public long? ParentLocationId { get; set; }
    public string? Notes { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

