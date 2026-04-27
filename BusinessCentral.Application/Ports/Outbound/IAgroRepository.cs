using BusinessCentral.Application.DTOs.Agro;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IAgroRepository
{
    Task<long> CreateLotAsync(int companyId, string kind, string code, string? name, DateTime startDate, int initialUnits, decimal? initialAvgWeightKg, string? notes);
    Task<List<AgroLotDTO>> ListLotsAsync(int companyId, string? kind = null, bool onlyOpen = false);
    Task<long> CreateFeedLogAsync(int companyId, long lotId, DateTime feedDate, int feedProductId, long? feedVariantId, decimal quantity, long? fromLocationId, decimal? unitCost, string? notes);
    Task<long> CreateMortalityLogAsync(int companyId, long lotId, DateTime mortalityDate, int units, decimal? avgWeightKg, string? notes);
    Task<long> CreateHarvestAsync(int companyId, long lotId, DateTime harvestDate, int outputProductId, long? outputVariantId, int units, decimal? totalWeightKg, long? toLocationId, string? notes);
    Task<AgroLotKpisDTO> GetLotKpisAsync(int companyId, long lotId);
}

