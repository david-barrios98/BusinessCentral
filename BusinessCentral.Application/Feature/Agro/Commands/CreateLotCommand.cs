using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Agro.Commands;

public sealed record CreateLotCommand(
    int CompanyId,
    string Kind,
    string Code,
    string? Name,
    DateTime StartDate,
    int InitialUnits,
    decimal? InitialAvgWeightKg,
    string? Notes
) : IRequest<Result<long>>;

