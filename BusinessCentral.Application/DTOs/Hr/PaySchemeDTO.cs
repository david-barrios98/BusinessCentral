namespace BusinessCentral.Application.DTOs.Hr;

public sealed class PaySchemeDTO
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public bool Active { get; set; }
}

