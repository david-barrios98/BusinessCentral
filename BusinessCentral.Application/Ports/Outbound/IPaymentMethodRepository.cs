using BusinessCentral.Application.DTOs.Config;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IPaymentMethodRepository
{
    Task<List<PaymentMethodDTO>> ListMethodsAsync(bool onlyActive = true);
    Task<PaymentMethodDTO?> GetMethodByIdAsync(int id);
    Task<int> UpsertMethodAsync(UpsertPaymentMethodRequestDTO request);
    Task DeleteMethodAsync(int id);
    Task<List<PaymentMethodDTO>> ListCompanyMethodsAsync(int companyId, bool onlyEnabled = true);
    Task<bool> SetCompanyMethodAsync(int companyId, string methodCode, bool enabled);
}

