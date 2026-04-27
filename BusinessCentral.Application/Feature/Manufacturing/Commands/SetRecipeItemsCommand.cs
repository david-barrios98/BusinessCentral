using BusinessCentral.Application.DTOs.Manufacturing;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Manufacturing.Commands;

public sealed record SetRecipeItemsCommand(int CompanyId, long RecipeId, List<RecipeItemUpsertDTO> Items)
    : IRequest<Result<bool>>;

