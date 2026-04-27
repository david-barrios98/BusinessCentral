using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Manufacturing.Commands;

public sealed class SetRecipeItemsHandler : IRequestHandler<SetRecipeItemsCommand, Result<bool>>
{
    private readonly IManufacturingRepository _repo;

    public SetRecipeItemsHandler(IManufacturingRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<bool>> Handle(SetRecipeItemsCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0 || request.RecipeId <= 0)
            return Result<bool>.Failure("CompanyId/RecipeId inválido.", "VALIDATION", "Validation");

        var ok = await _repo.SetRecipeItemsAsync(request.CompanyId, request.RecipeId, request.Items ?? new());
        return Result<bool>.Success(ok);
    }
}

