using BusinessCentral.Application.DTOs.Business;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Business.StorageLocations.Queries;

public sealed record ListStorageLocationsQuery(int CompanyId, int? FacilityId = null, bool OnlyActive = true, int Page = 1, int PageSize = 50)
    : IRequest<Result<PagedResult<StorageLocationDTO>>>;

