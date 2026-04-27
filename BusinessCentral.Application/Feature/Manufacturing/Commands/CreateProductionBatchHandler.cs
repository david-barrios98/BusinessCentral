using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Manufacturing.Commands;

public sealed class CreateProductionBatchHandler : IRequestHandler<CreateProductionBatchCommand, Result<long>>
{
    private readonly IManufacturingRepository _repo;

    public CreateProductionBatchHandler(IManufacturingRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<long>> Handle(CreateProductionBatchCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0 || request.OutputProductId <= 0 || request.RecipeId <= 0)
            return Result<long>.Failure("CompanyId/RecipeId/OutputProductId inválido.", "VALIDATION", "Validation");

        if (request.QuantityProduced <= 0)
            return Result<long>.Failure("QuantityProduced inválido.", "VALIDATION", "Validation");

        var id = await _repo.CreateBatchAsync(
            request.CompanyId,
            request.RecipeId,
            request.OutputProductId,
            request.OutputVariantId,
            request.QuantityProduced,
            request.ToLocationId,
            request.BatchDate,
            request.Notes
        );

        return Result<long>.Success(id);
    }
}

