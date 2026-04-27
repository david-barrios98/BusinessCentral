using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.Feature.Commerce.Purchasing.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Commerce;

[Authorize]
[RequiresModule("COMMERCE")]
[Route("api/v1/secure/commerce/purchases")]
public sealed class PurchasesController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public PurchasesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public sealed class CreateReceiptRequest
    {
        public long? SupplierId { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string? SupplierInvoiceNumber { get; set; }
        public long? DefaultToLocationId { get; set; }
        public int? CreatedByUserId { get; set; }
    }

    [HttpPost("receipts")]
    public async Task<IActionResult> CreateReceipt([FromBody] CreateReceiptRequest req)
    {
        var result = await _mediator.Send(new CreatePurchaseReceiptCommand(
            CompanyId,
            req.SupplierId,
            req.ReceiptDate,
            req.SupplierInvoiceNumber,
            req.DefaultToLocationId,
            req.CreatedByUserId
        ));
        return ProcessResult(result);
    }

    public sealed class AddReceiptLineRequest
    {
        public int ProductId { get; set; }
        public long? VariantId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public long? ToLocationId { get; set; }
        public string? Notes { get; set; }
    }

    [HttpPost("receipts/{receiptId:long}/lines")]
    public async Task<IActionResult> AddLine([FromRoute] long receiptId, [FromBody] AddReceiptLineRequest req)
    {
        var result = await _mediator.Send(new AddPurchaseReceiptLineCommand(
            CompanyId,
            receiptId,
            req.ProductId,
            req.VariantId,
            req.Quantity,
            req.UnitCost,
            req.ToLocationId,
            req.Notes
        ));
        return ProcessResult(result);
    }

    [HttpPost("receipts/{receiptId:long}/post")]
    public async Task<IActionResult> Post([FromRoute] long receiptId)
    {
        var result = await _mediator.Send(new PostPurchaseReceiptCommand(CompanyId, receiptId));
        return ProcessResult(result);
    }
}

