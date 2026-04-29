using BusinessCentral.Application.DTOs.Config;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IApplicationCompanyRepository
{
    Task<List<ApplicationCompanyDTO>> ListByCompanyAsync(int companyId);
    Task<int> UpsertAsync(int companyId, UpsertApplicationCompanyRequestDTO request);
    Task DeleteAsync(int companyId, int id);
}
