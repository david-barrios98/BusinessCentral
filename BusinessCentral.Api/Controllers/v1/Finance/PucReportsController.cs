using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.Feature.Finance.Puc.Reports.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Finance;

[Authorize]
[RequiresModule("FIN")]
[Route("api/v1/secure/finance/puc/reports")]
public sealed class PucReportsController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public PucReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("trial-balance")]
    public async Task<IActionResult> TrialBalance([FromQuery] DateTime fromDateUtc, [FromQuery] DateTime toDateUtc)
    {
        var result = await _mediator.Send(new GetTrialBalanceQuery(CompanyId, fromDateUtc, toDateUtc));
        return ProcessResult(result);
    }

    [HttpGet("income-statement")]
    public async Task<IActionResult> IncomeStatement([FromQuery] DateTime fromDateUtc, [FromQuery] DateTime toDateUtc)
    {
        var result = await _mediator.Send(new GetIncomeStatementQuery(CompanyId, fromDateUtc, toDateUtc));
        return ProcessResult(result);
    }

    [HttpGet("balance-sheet")]
    public async Task<IActionResult> BalanceSheet([FromQuery] DateTime toDateUtc)
    {
        var result = await _mediator.Send(new GetBalanceSheetQuery(CompanyId, toDateUtc));
        return ProcessResult(result);
    }
}

