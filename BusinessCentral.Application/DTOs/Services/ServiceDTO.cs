namespace BusinessCentral.Application.DTOs.Services;

public sealed class ServiceDTO
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public bool Active { get; set; }
}

