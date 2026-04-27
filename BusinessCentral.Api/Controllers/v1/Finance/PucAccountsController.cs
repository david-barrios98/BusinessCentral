using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.Feature.Finance.Puc.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Finance;

[Authorize]
[RequiresModule("FIN")]
[Route("api/v1/secure/finance/puc/accounts")]
public sealed class PucAccountsController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public PucAccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool onlyActive = true, [FromQuery] string? q = null)
    {
        var result = await _mediator.Send(new ListAccountsQuery(CompanyId, onlyActive, q));
        return ProcessResult(result);
    }
}

