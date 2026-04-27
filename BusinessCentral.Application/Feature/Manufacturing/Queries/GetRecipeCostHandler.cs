using BusinessCentral.Application.DTOs.Manufacturing;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Manufacturing.Queries;

public sealed class GetRecipeCostHandler : IRequestHandler<GetRecipeCostQuery, Result<RecipeCostReportDTO>>
{
    private readonly IManufacturingRepository _repo;

    public GetRecipeCostHandler(IManufacturingRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<RecipeCostReportDTO>> Handle(GetRecipeCostQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0 || request.RecipeId <= 0)
            return Result<RecipeCostReportDTO>.Failure("CompanyId/RecipeId inválido.", "VALIDATION", "Validation");

        if (request.Quantity <= 0)
            return Result<RecipeCostReportDTO>.Failure("Quantity inválida.", "VALIDATION", "Validation");

        var data = await _repo.GetRecipeCostAsync(request.CompanyId, request.RecipeId, request.Quantity);
        return Result<RecipeCostReportDTO>.Success(data);
    }
}

