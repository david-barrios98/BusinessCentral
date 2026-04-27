using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.Feature.Commerce.Variants.Commands;
using BusinessCentral.Application.Feature.Commerce.Variants.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Commerce;

[Authorize]
[RequiresModule("COMMERCE")]
[Route("api/v1/secure/commerce/product-variants")]
public sealed class ProductVariantsController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public ProductVariantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int? productId = null,
        [FromQuery] bool onlyActive = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? q = null)
    {
        var result = await _mediator.Send(new ListVariantsQuery(CompanyId, productId, onlyActive, page, pageSize, q));
        return ProcessResult(result);
    }

    public sealed class UpsertRequest
    {
        public long? Id { get; set; }
        public int ProductId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public string? VariantName { get; set; }
        public decimal? PriceOverride { get; set; }
        public decimal? CostOverride { get; set; }
        public bool Active { get; set; } = true;
    }

    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] UpsertRequest req)
    {
        var result = await _mediator.Send(new UpsertVariantCommand(
            CompanyId,
            req.Id,
            req.ProductId,
            req.Sku,
            req.Barcode,
            req.VariantName,
            req.PriceOverride,
            req.CostOverride,
            req.Active,
            UserId == 0 ? null : UserId
        ));
        return ProcessResult(result);
    }
}

