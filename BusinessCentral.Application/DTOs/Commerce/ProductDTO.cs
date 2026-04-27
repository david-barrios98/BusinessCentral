namespace BusinessCentral.Application.DTOs.Commerce;

public sealed class ProductDTO
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public decimal Price { get; set; }
    public bool Active { get; set; }
}

