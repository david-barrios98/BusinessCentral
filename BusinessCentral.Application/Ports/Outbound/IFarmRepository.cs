using BusinessCentral.Application.DTOs.Farm;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IFarmRepository
{
    Task<bool> UpsertZoneAsync(int companyId, string code, string name, bool active);
    Task<List<FarmZoneDTO>> ListZonesAsync(int companyId, bool onlyActive);

    Task<long> CreateHarvestLotAsync(int companyId, HarvestLotDTO dto);
    Task<List<HarvestLotDTO>> ListHarvestLotsAsync(int companyId, DateTime? fromDate, DateTime? toDate, int? zoneId);

    Task<long> CreateProcessStepAsync(int companyId, CoffeeProcessStepDTO dto);
    Task<List<CoffeeProcessStepDTO>> ListProcessStepsAsync(int companyId, long harvestLotId);
}

