using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.Feature.Finance.Reports.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Finance;

[Authorize]
[RequiresModule("FIN")]
[Route("api/v1/secure/finance/reports")]
public sealed class FinanceReportsController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public FinanceReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> Summary([FromQuery] DateTime fromDateUtc, [FromQuery] DateTime toDateUtc)
    {
        var result = await _mediator.Send(new GetFinancialSummaryQuery(CompanyId, fromDateUtc, toDateUtc));
        return ProcessResult(result);
    }

    [HttpGet("pnl")]
    public async Task<IActionResult> ProfitAndLoss([FromQuery] DateTime fromDateUtc, [FromQuery] DateTime toDateUtc)
    {
        var result = await _mediator.Send(new GetPnLQuery(CompanyId, fromDateUtc, toDateUtc));
        return ProcessResult(result);
    }

    [HttpGet("tax/co")]
    public async Task<IActionResult> TaxSummaryCo([FromQuery] DateTime fromDateUtc, [FromQuery] DateTime toDateUtc)
    {
        var result = await _mediator.Send(new GetTaxSummaryCoQuery(CompanyId, fromDateUtc, toDateUtc));
        return ProcessResult(result);
    }

    [HttpGet("renta/co/{year:int}")]
    public async Task<IActionResult> RentaAnnualCo([FromRoute] int year)
    {
        var result = await _mediator.Send(new GetRentaAnnualCoQuery(CompanyId, year));
        return ProcessResult(result);
    }
}

