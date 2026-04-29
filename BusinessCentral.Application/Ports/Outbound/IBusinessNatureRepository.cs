using BusinessCentral.Application.DTOs.Config;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IBusinessNatureRepository
{
    Task<List<BusinessNatureDTO>> ListAsync(bool onlyActive = true);
    Task<BusinessNatureDTO?> GetByIdAsync(int id);
    Task<int> UpsertAsync(UpsertBusinessNatureRequestDTO request);
    Task DeleteAsync(int id);
}

