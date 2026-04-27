using BusinessCentral.Application.DTOs.Farm;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Farm.ProcessSteps;

public sealed class ListProcessStepsHandler : IRequestHandler<ListProcessStepsQuery, Result<List<CoffeeProcessStepDTO>>>
{
    private readonly IFarmRepository _farm;

    public ListProcessStepsHandler(IFarmRepository farm)
    {
        _farm = farm;
    }

    public async Task<Result<List<CoffeeProcessStepDTO>>> Handle(ListProcessStepsQuery request, CancellationToken cancellationToken)
    {
        var list = await _farm.ListProcessStepsAsync(request.CompanyId, request.HarvestLotId);
        return Result<List<CoffeeProcessStepDTO>>.Success(list);
    }
}

