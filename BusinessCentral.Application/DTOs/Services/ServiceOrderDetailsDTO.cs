namespace BusinessCentral.Application.DTOs.Services;

public sealed class ServiceOrderDetailsDTO
{
    public ServiceOrderDTO? Order { get; set; }
    public List<ServiceOrderLineDTO> Lines { get; set; } = new();
}

