using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.Feature.Commerce.Pos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Commerce;

[Authorize]
[RequiresModule("POS")]
[Route("api/v1/secure/pos")]
public sealed class PosController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public PosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public sealed class OpenCashRequest
    {
        public int? OpenedByUserId { get; set; }
        public decimal OpeningAmount { get; set; } = 0;
    }

    [HttpPost("cash-sessions")]
    public async Task<IActionResult> OpenCash([FromBody] OpenCashRequest req)
    {
        var result = await _mediator.Send(new CreateCashSessionCommand(CompanyId, req.OpenedByUserId, req.OpeningAmount));
        return ProcessResult(result);
    }

    public sealed class CreateTicketRequest
    {
        public long? CashSessionId { get; set; }
        public string? FulfillmentMethodCode { get; set; }
        public string? FulfillmentDetails { get; set; }
    }

    [HttpPost("tickets")]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest req)
    {
        var result = await _mediator.Send(new CreatePosTicketCommand(CompanyId, req.CashSessionId, req.FulfillmentMethodCode, req.FulfillmentDetails));
        return ProcessResult(result);
    }

    [HttpGet("tickets/{ticketId:long}")]
    public async Task<IActionResult> GetTicket([FromRoute] long ticketId)
    {
        var result = await _mediator.Send(new GetPosTicketReceiptQuery(CompanyId, ticketId));
        return ProcessResult(result);
    }

    public sealed class AddTicketLineRequest
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
    }

    [HttpPost("tickets/{ticketId:long}/lines")]
    public async Task<IActionResult> AddLine([FromRoute] long ticketId, [FromBody] AddTicketLineRequest req)
    {
        var result = await _mediator.Send(new AddPosTicketLineCommand(
            CompanyId,
            ticketId,
            req.ProductId,
            req.Quantity,
            req.UnitPrice
        ));
        return ProcessResult(result);
    }

    public sealed class PayTicketRequest
    {
        public string Method { get; set; } = "CASH";
        public decimal Amount { get; set; }
    }

    [HttpPost("tickets/{ticketId:long}/pay")]
    public async Task<IActionResult> Pay([FromRoute] long ticketId, [FromBody] PayTicketRequest req)
    {
        var result = await _mediator.Send(new PayPosTicketCommand(CompanyId, ticketId, req.Method, req.Amount));
        return ProcessResult(result);
    }
}

