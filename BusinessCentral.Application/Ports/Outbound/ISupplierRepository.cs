using BusinessCentral.Application.DTOs.Commerce;

namespace BusinessCentral.Application.Ports.Outbound;

public interface ISupplierRepository
{
    Task<long> UpsertAsync(int companyId, long? id, string name, string? documentNumber, string? phone, string? email, string? notes, bool active);
    Task<List<SupplierDTO>> ListAsync(int companyId, bool onlyActive = true, string? q = null);
}

