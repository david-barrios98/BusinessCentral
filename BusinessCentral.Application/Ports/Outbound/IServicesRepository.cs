using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.DTOs.Services;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IServicesRepository
{
    Task<bool> UpsertServiceAsync(int companyId, string code, string name, decimal basePrice, bool active);
    Task<List<ServiceDTO>> ListServicesAsync(int companyId, bool onlyActive);
    Task<PagedResult<ServiceOrderDTO>> ListServiceOrdersAsync(int companyId, string? status, int page, int pageSize);

    Task<long> CreateServiceOrderAsync(int companyId, string? vehicleType, string? plate, string? customerName, string? fulfillmentMethodCode, string? fulfillmentDetails);
    Task<long> AddServiceOrderLineAsync(int companyId, long orderId, int serviceId, decimal quantity, decimal unitPrice, int? employeeUserId);
    Task<ServiceOrderDetailsDTO?> GetServiceOrderAsync(int companyId, long orderId);

    // --- Servicios: agendas/coberturas/turnos (config opcional) ---
    Task<ServiceCompanySettingsDTO> GetCompanySettingsAsync(int companyId);
    Task<bool> UpdateCompanySettingsAsync(int companyId, UpdateServiceCompanySettingsRequest request);

    Task<List<ServiceCoverageAreaDTO>> ListCoverageAreasAsync(int companyId, bool onlyActive = true);
    Task<int> UpsertCoverageAreaAsync(int companyId, UpsertServiceCoverageAreaRequest request);
    Task DeleteCoverageAreaAsync(int companyId, int id);

    Task<List<ServiceShiftTemplateDTO>> ListShiftTemplatesAsync(int companyId, bool onlyActive = true);
    Task<int> UpsertShiftTemplateAsync(int companyId, UpsertServiceShiftTemplateRequest request);
    Task DeleteShiftTemplateAsync(int companyId, int id);
}

