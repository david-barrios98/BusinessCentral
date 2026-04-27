using BusinessCentral.Application.DTOs.Agro;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Agro.Queries;

public sealed class ListLotsHandler : IRequestHandler<ListLotsQuery, Result<PagedResult<AgroLotDTO>>>
{
    private readonly IAgroRepository _repo;

    public ListLotsHandler(IAgroRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<PagedResult<AgroLotDTO>>> Handle(ListLotsQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<PagedResult<AgroLotDTO>>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 500 ? 50 : request.PageSize;

        var data = await _repo.ListLotsAsync(request.CompanyId, request.Kind, request.OnlyOpen, page, pageSize);
        return Result<PagedResult<AgroLotDTO>>.Success(data);
    }
}

