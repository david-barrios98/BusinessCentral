using BusinessCentral.Application.DTOs.Services;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IServicesRepository
{
    Task<bool> UpsertServiceAsync(int companyId, string code, string name, decimal basePrice, bool active);
    Task<List<ServiceDTO>> ListServicesAsync(int companyId, bool onlyActive);

    Task<long> CreateServiceOrderAsync(int companyId, string? vehicleType, string? plate, string? customerName, string? fulfillmentMethodCode, string? fulfillmentDetails);
    Task<long> AddServiceOrderLineAsync(int companyId, long orderId, int serviceId, decimal quantity, decimal unitPrice, int? employeeUserId);
    Task<ServiceOrderDetailsDTO?> GetServiceOrderAsync(int companyId, long orderId);
}

