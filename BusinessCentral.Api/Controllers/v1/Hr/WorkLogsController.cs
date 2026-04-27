using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Hr.WorkLogs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Hr;

[Authorize]
[RequiresModule("HR")]
[Route("api/v1/secure/hr/work-logs")]
public sealed class WorkLogsController : HrControllerBase
{
    private readonly IMediator _mediator;

    public WorkLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? userId = null, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        var result = await _mediator.Send(new ListWorkLogsQuery(CompanyId, userId, from, to));
        return ProcessResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WorkLogDTO dto)
    {
        dto.CompanyId = CompanyId;
        var result = await _mediator.Send(new CreateWorkLogCommand(CompanyId, dto));
        return ProcessResult(result);
    }
}

