using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Farm.ProcessSteps;

public sealed class CreateProcessStepHandler : IRequestHandler<CreateProcessStepCommand, Result<long>>
{
    private readonly IFarmRepository _farm;

    public CreateProcessStepHandler(IFarmRepository farm)
    {
        _farm = farm;
    }

    public async Task<Result<long>> Handle(CreateProcessStepCommand request, CancellationToken cancellationToken)
    {
        if (request.Step.HarvestLotId <= 0 || string.IsNullOrWhiteSpace(request.Step.StepType))
            return Result<long>.Failure("HarvestLotId y StepType son requeridos.", "FARM_STEP_VALIDATION", "Validation");

        var id = await _farm.CreateProcessStepAsync(request.CompanyId, request.Step);
        return id > 0
            ? Result<long>.Success(id)
            : Result<long>.Failure("No se pudo crear el paso.", "FARM_STEP_CREATE_FAILED", "Conflict");
    }
}

