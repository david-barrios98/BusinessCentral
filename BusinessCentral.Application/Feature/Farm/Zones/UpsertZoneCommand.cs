using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Farm.Zones;

public sealed record UpsertZoneCommand(
    int CompanyId,
    string Code,
    string Name,
    bool Active
) : IRequest<Result<bool>>;

