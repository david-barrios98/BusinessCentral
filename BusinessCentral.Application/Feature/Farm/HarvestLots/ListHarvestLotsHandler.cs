using BusinessCentral.Application.DTOs.Farm;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Farm.HarvestLots;

public sealed class ListHarvestLotsHandler : IRequestHandler<ListHarvestLotsQuery, Result<List<HarvestLotDTO>>>
{
    private readonly IFarmRepository _farm;

    public ListHarvestLotsHandler(IFarmRepository farm)
    {
        _farm = farm;
    }

    public async Task<Result<List<HarvestLotDTO>>> Handle(ListHarvestLotsQuery request, CancellationToken cancellationToken)
    {
        var list = await _farm.ListHarvestLotsAsync(request.CompanyId, request.FromDate, request.ToDate, request.ZoneId);
        return Result<List<HarvestLotDTO>>.Success(list);
    }
}

