namespace BusinessCentral.Application.DTOs.Services;

public sealed class ServiceOrderDTO
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public DateTime OrderDate { get; set; }
    public string? VehicleType { get; set; }
    public string? Plate { get; set; }
    public string? CustomerName { get; set; }
    public string? FulfillmentMethodCode { get; set; }
    public string? FulfillmentDetails { get; set; }
    public string Status { get; set; } = "open";
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
}

