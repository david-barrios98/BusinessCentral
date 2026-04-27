using BusinessCentral.Application.DTOs.Manufacturing;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Manufacturing.Queries;

public sealed record GetRecipeCostQuery(int CompanyId, long RecipeId, decimal Quantity)
    : IRequest<Result<RecipeCostReportDTO>>;

