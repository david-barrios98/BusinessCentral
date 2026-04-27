using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Agro.Commands;

public sealed record CreateMortalityLogCommand(
    int CompanyId,
    long LotId,
    DateTime MortalityDate,
    int Units,
    decimal? AvgWeightKg,
    string? Notes
) : IRequest<Result<long>>;

