using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Farm.HarvestLots;

public sealed class CreateHarvestLotHandler : IRequestHandler<CreateHarvestLotCommand, Result<long>>
{
    private readonly IFarmRepository _farm;

    public CreateHarvestLotHandler(IFarmRepository farm)
    {
        _farm = farm;
    }

    public async Task<Result<long>> Handle(CreateHarvestLotCommand request, CancellationToken cancellationToken)
    {
        if (request.Lot.QuantityKg <= 0 || string.IsNullOrWhiteSpace(request.Lot.ProductForm))
            return Result<long>.Failure("ProductForm y QuantityKg son requeridos.", "FARM_LOT_VALIDATION", "Validation");

        var id = await _farm.CreateHarvestLotAsync(request.CompanyId, request.Lot);
        return id > 0
            ? Result<long>.Success(id)
            : Result<long>.Failure("No se pudo crear el lote.", "FARM_LOT_CREATE_FAILED", "Conflict");
    }
}

