using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Variants.Commands;

public sealed record UpsertVariantCommand(
    int CompanyId,
    long? Id,
    int ProductId,
    string Sku,
    string? Barcode,
    string? VariantName,
    decimal? PriceOverride,
    decimal? CostOverride,
    bool Active,
    int? PerformedByUserId
) : IRequest<Result<long>>;

