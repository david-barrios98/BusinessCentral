using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Agro.Commands;

public sealed record CreateFeedLogCommand(
    int CompanyId,
    long LotId,
    DateTime FeedDate,
    int FeedProductId,
    long? FeedVariantId,
    decimal Quantity,
    long? FromLocationId,
    decimal? UnitCost,
    string? Notes
) : IRequest<Result<long>>;

