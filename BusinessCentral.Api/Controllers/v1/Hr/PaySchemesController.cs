using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.Feature.Hr.PaySchemes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Hr;

[Authorize]
[RequiresModule("HR")]
[Route("api/v1/secure/hr/pay-schemes")]
public sealed class PaySchemesController : HrControllerBase
{
    private readonly IMediator _mediator;

    public PaySchemesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool onlyActive = true)
    {
        var result = await _mediator.Send(new ListPaySchemesQuery(CompanyId, onlyActive));
        return ProcessResult(result);
    }

    public sealed class UpsertPaySchemeRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public bool Active { get; set; } = true;
    }

    [HttpPut]
    public async Task<IActionResult> Upsert([FromBody] UpsertPaySchemeRequest req)
    {
        var result = await _mediator.Send(new UpsertPaySchemeCommand(
            CompanyId,
            req.Code,
            req.Name,
            req.Unit,
            req.Active
        ));
        return ProcessResult(result);
    }
}

