using BusinessCentral.Application.DTOs.Business;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Business.StorageLocations.Queries;

public sealed class ListStorageLocationsHandler : IRequestHandler<ListStorageLocationsQuery, Result<PagedResult<StorageLocationDTO>>>
{
    private readonly IStorageLocationRepository _repo;

    public ListStorageLocationsHandler(IStorageLocationRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<PagedResult<StorageLocationDTO>>> Handle(ListStorageLocationsQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<PagedResult<StorageLocationDTO>>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 500 ? 50 : request.PageSize;

        var data = await _repo.ListAsync(request.CompanyId, request.FacilityId, request.OnlyActive, page, pageSize);
        return Result<PagedResult<StorageLocationDTO>>.Success(data);
    }
}

