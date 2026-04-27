using BusinessCentral.Application.DTOs.Agro;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Agro.Queries;

public sealed class ListLotsHandler : IRequestHandler<ListLotsQuery, Result<List<AgroLotDTO>>>
{
    private readonly IAgroRepository _repo;

    public ListLotsHandler(IAgroRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<List<AgroLotDTO>>> Handle(ListLotsQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<List<AgroLotDTO>>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        var data = await _repo.ListLotsAsync(request.CompanyId, request.Kind, request.OnlyOpen);
        return Result<List<AgroLotDTO>>.Success(data);
    }
}

