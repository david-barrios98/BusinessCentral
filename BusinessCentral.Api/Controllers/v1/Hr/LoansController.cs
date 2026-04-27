using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Hr.Loans;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Hr;

[Authorize]
[RequiresModule("HR")]
[Route("api/v1/secure/hr/loans")]
public sealed class LoansController : HrControllerBase
{
    private readonly IMediator _mediator;

    public LoansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? userId = null, [FromQuery] string? status = null)
    {
        var result = await _mediator.Send(new ListLoanAdvancesQuery(CompanyId, userId, status));
        return ProcessResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LoanAdvanceDTO dto)
    {
        dto.CompanyId = CompanyId;
        var result = await _mediator.Send(new CreateLoanAdvanceCommand(CompanyId, dto));
        return ProcessResult(result);
    }
}

