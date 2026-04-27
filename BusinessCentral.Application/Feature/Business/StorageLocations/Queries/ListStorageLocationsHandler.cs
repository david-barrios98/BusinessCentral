using BusinessCentral.Application.DTOs.Business;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Business.StorageLocations.Queries;

public sealed class ListStorageLocationsHandler : IRequestHandler<ListStorageLocationsQuery, Result<List<StorageLocationDTO>>>
{
    private readonly IStorageLocationRepository _repo;

    public ListStorageLocationsHandler(IStorageLocationRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<List<StorageLocationDTO>>> Handle(ListStorageLocationsQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<List<StorageLocationDTO>>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        var data = await _repo.ListAsync(request.CompanyId, request.FacilityId, request.OnlyActive);
        return Result<List<StorageLocationDTO>>.Success(data);
    }
}

