using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Manufacturing.Commands;

public sealed record UpsertRecipeCommand(
    int CompanyId,
    long? Id,
    int OutputProductId,
    long? OutputVariantId,
    decimal OutputQuantity,
    string? Notes,
    bool Active
) : IRequest<Result<long>>;

