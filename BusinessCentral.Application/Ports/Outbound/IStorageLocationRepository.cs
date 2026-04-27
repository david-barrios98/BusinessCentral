using BusinessCentral.Application.DTOs.Business;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IStorageLocationRepository
{
    Task<long> UpsertAsync(int companyId, long? id, int? facilityId, string code, string name, string type, long? parentLocationId, string? notes, bool active);
    Task<List<StorageLocationDTO>> ListAsync(int companyId, int? facilityId = null, bool onlyActive = true);
}

