using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Manufacturing.Commands;

public sealed class UpsertRecipeHandler : IRequestHandler<UpsertRecipeCommand, Result<long>>
{
    private readonly IManufacturingRepository _repo;

    public UpsertRecipeHandler(IManufacturingRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<long>> Handle(UpsertRecipeCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0 || request.OutputProductId <= 0)
            return Result<long>.Failure("CompanyId/OutputProductId inválido.", "VALIDATION", "Validation");

        if (request.OutputQuantity <= 0)
            return Result<long>.Failure("OutputQuantity inválido.", "VALIDATION", "Validation");

        var id = await _repo.UpsertRecipeAsync(
            request.CompanyId,
            request.Id,
            request.OutputProductId,
            request.OutputVariantId,
            request.OutputQuantity,
            request.Notes,
            request.Active
        );

        return Result<long>.Success(id);
    }
}

