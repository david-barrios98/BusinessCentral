using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Products;

public sealed record UpsertProductCommand(
    int CompanyId,
    string Sku,
    string Name,
    string? Unit,
    decimal Price,
    bool Active,
    int? PerformedByUserId
) : IRequest<Result<bool>>;

