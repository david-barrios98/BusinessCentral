using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Farm.Zones;

public sealed class UpsertZoneHandler : IRequestHandler<UpsertZoneCommand, Result<bool>>
{
    private readonly IFarmRepository _farm;

    public UpsertZoneHandler(IFarmRepository farm)
    {
        _farm = farm;
    }

    public async Task<Result<bool>> Handle(UpsertZoneCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.Name))
            return Result<bool>.Failure("Code y Name son requeridos.", "FARM_ZONE_VALIDATION", "Validation");

        var ok = await _farm.UpsertZoneAsync(request.CompanyId, request.Code, request.Name, request.Active);
        return ok
            ? Result<bool>.Success(true)
            : Result<bool>.Failure("No se pudo guardar la zona.", "FARM_ZONE_UPSERT_FAILED", "Conflict");
    }
}

