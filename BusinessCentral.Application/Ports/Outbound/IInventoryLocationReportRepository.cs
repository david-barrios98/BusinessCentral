using BusinessCentral.Application.DTOs.Commerce;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IInventoryLocationReportRepository
{
    Task<List<InventoryByLocationRowDTO>> GetInventoryByLocationAsync(int companyId, DateTime asOfUtc, long? locationId = null);
}

