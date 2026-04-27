using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Manufacturing.Commands;

public sealed record CreateProductionBatchCommand(
    int CompanyId,
    long RecipeId,
    int OutputProductId,
    long? OutputVariantId,
    decimal QuantityProduced,
    long? ToLocationId,
    DateTime BatchDate,
    string? Notes
) : IRequest<Result<long>>;

