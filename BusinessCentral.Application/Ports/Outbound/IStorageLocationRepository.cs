using BusinessCentral.Application.DTOs.Business;
using BusinessCentral.Application.DTOs.Common;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IStorageLocationRepository
{
    Task<long> UpsertAsync(int companyId, long? id, int? facilityId, string code, string name, string type, long? parentLocationId, string? notes, bool active, int? performedByUserId);
    Task<PagedResult<StorageLocationDTO>> ListAsync(int companyId, int? facilityId = null, bool onlyActive = true, int page = 1, int pageSize = 50);
}

