using BusinessCentral.Application.DTOs.Config;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IFacilityTypeRepository
{
    Task<List<FacilityTypeDTO>> ListAsync(bool onlyActive = true);
    Task<FacilityTypeDTO?> GetByIdAsync(int id);
    Task<int> UpsertAsync(UpsertFacilityTypeRequestDTO request);
    Task DeleteAsync(int id);
}

