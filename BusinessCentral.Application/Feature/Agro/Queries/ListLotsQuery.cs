using BusinessCentral.Application.DTOs.Agro;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Agro.Queries;

public sealed record ListLotsQuery(int CompanyId, string? Kind = null, bool OnlyOpen = false, int Page = 1, int PageSize = 50)
    : IRequest<Result<PagedResult<AgroLotDTO>>>;

