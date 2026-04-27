using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Agro.Commands;

public sealed class CreateHarvestHandler : IRequestHandler<CreateHarvestCommand, Result<long>>
{
    private readonly IAgroRepository _repo;

    public CreateHarvestHandler(IAgroRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<long>> Handle(CreateHarvestCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0 || request.LotId <= 0 || request.OutputProductId <= 0)
            return Result<long>.Failure("Parámetros inválidos.", "VALIDATION", "Validation");

        if (request.Units <= 0)
            return Result<long>.Failure("Units inválidas.", "VALIDATION", "Validation");

        var id = await _repo.CreateHarvestAsync(
            request.CompanyId,
            request.LotId,
            request.HarvestDate,
            request.OutputProductId,
            request.OutputVariantId,
            request.Units,
            request.TotalWeightKg,
            request.ToLocationId,
            request.Notes
        );

        return Result<long>.Success(id);
    }
}

