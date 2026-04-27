namespace BusinessCentral.Application.DTOs.Commerce;

public sealed class SupplierDTO
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? DocumentNumber { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Notes { get; set; }
    public bool Active { get; set; }
}

