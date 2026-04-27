namespace BusinessCentral.Application.DTOs.Services;

public sealed class ServiceOrderLineDTO
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public int ServiceId { get; set; }
    public string? ServiceName { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public int? EmployeeUserId { get; set; }
}

