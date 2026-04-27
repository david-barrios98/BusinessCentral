using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Hr.Deductions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Hr;

[Authorize]
[RequiresModule("HR")]
[Route("api/v1/secure/hr/deductions")]
public sealed class DeductionsController : HrControllerBase
{
    private readonly IMediator _mediator;

    public DeductionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? userId = null, [FromQuery] string? type = null)
    {
        var result = await _mediator.Send(new ListDeductionsQuery(CompanyId, userId, type));
        return ProcessResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DeductionDTO dto)
    {
        dto.CompanyId = CompanyId;
        var result = await _mediator.Send(new CreateDeductionCommand(CompanyId, dto));
        return ProcessResult(result);
    }
}

