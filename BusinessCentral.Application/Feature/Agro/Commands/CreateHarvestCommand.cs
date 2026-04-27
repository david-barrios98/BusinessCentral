using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Agro.Commands;

public sealed record CreateHarvestCommand(
    int CompanyId,
    long LotId,
    DateTime HarvestDate,
    int OutputProductId,
    long? OutputVariantId,
    int Units,
    decimal? TotalWeightKg,
    long? ToLocationId,
    string? Notes
) : IRequest<Result<long>>;

