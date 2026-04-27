using BusinessCentral.Application.DTOs.Agro;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Agro.Queries;

public sealed class GetLotKpisHandler : IRequestHandler<GetLotKpisQuery, Result<AgroLotKpisDTO>>
{
    private readonly IAgroRepository _repo;

    public GetLotKpisHandler(IAgroRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<AgroLotKpisDTO>> Handle(GetLotKpisQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0 || request.LotId <= 0)
            return Result<AgroLotKpisDTO>.Failure("Parámetros inválidos.", "VALIDATION", "Validation");

        var data = await _repo.GetLotKpisAsync(request.CompanyId, request.LotId);
        return Result<AgroLotKpisDTO>.Success(data);
    }
}

