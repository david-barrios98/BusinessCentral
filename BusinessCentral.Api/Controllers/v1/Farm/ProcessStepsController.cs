using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.DTOs.Farm;
using BusinessCentral.Application.Feature.Farm.ProcessSteps;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Farm;

[Authorize]
[RequiresModule("FARM")]
[Route("api/v1/secure/farm/process-steps")]
public sealed class ProcessStepsController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public ProcessStepsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{harvestLotId:long}")]
    public async Task<IActionResult> List([FromRoute] long harvestLotId)
    {
        var result = await _mediator.Send(new ListProcessStepsQuery(CompanyId, harvestLotId));
        return ProcessResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CoffeeProcessStepDTO dto)
    {
        dto.CompanyId = CompanyId;
        var result = await _mediator.Send(new CreateProcessStepCommand(CompanyId, dto));
        return ProcessResult(result);
    }
}

