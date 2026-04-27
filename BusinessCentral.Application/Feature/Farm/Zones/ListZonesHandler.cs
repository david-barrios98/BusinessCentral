using BusinessCentral.Application.DTOs.Farm;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Farm.Zones;

public sealed class ListZonesHandler : IRequestHandler<ListZonesQuery, Result<List<FarmZoneDTO>>>
{
    private readonly IFarmRepository _farm;

    public ListZonesHandler(IFarmRepository farm)
    {
        _farm = farm;
    }

    public async Task<Result<List<FarmZoneDTO>>> Handle(ListZonesQuery request, CancellationToken cancellationToken)
    {
        var list = await _farm.ListZonesAsync(request.CompanyId, request.OnlyActive);
        return Result<List<FarmZoneDTO>>.Success(list);
    }
}

