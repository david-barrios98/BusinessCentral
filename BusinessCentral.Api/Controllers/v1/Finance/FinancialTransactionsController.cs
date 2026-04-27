using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Finance.Transactions.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Finance;

[Authorize]
[RequiresModule("FIN")]
[Route("api/v1/secure/finance/transactions")]
public sealed class FinancialTransactionsController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public FinancialTransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFinancialTransactionDTO dto)
    {
        var result = await _mediator.Send(new CreateFinancialTransactionCommand(CompanyId, dto));
        return ProcessResult(result);
    }
}

