using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Business.StorageLocations.Commands;

public sealed record UpsertStorageLocationCommand(
    int CompanyId,
    long? Id,
    int? FacilityId,
    string Code,
    string Name,
    string Type,
    long? ParentLocationId,
    string? Notes,
    bool Active,
    int? PerformedByUserId
) : IRequest<Result<long>>;

