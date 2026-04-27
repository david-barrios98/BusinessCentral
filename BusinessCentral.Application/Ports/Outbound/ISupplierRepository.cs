using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.DTOs.Common;

namespace BusinessCentral.Application.Ports.Outbound;

public interface ISupplierRepository
{
    Task<long> UpsertAsync(int companyId, long? id, string name, string? documentNumber, string? phone, string? email, string? notes, bool active);
    Task<PagedResult<SupplierDTO>> ListAsync(int companyId, bool onlyActive = true, int page = 1, int pageSize = 50, string? q = null);
}

