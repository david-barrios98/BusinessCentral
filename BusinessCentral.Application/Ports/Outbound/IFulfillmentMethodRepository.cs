using BusinessCentral.Application.DTOs.Config;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IFulfillmentMethodRepository
{
    Task<List<FulfillmentMethodDTO>> ListMethodsAsync(bool onlyActive = true);
    Task<FulfillmentMethodDTO?> GetMethodByIdAsync(int id);
    Task<int> UpsertMethodAsync(UpsertFulfillmentMethodRequestDTO request);
    Task DeleteMethodAsync(int id);
    Task<List<FulfillmentMethodDTO>> ListCompanyMethodsAsync(int companyId, bool onlyEnabled = true);
    Task<bool> SetCompanyMethodAsync(int companyId, string methodCode, bool enabled);
}

