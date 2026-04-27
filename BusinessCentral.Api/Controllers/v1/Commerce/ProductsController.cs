using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.Feature.Commerce.Products;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Commerce;

[Authorize]
[RequiresModule("COMMERCE")]
[Route("api/v1/secure/commerce/products")]
public sealed class ProductsController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] bool onlyActive = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? q = null)
    {
        var result = await _mediator.Send(new ListProductsQuery(CompanyId, onlyActive, page, pageSize, q));
        return ProcessResult(result);
    }

    public sealed class UpsertProductRequest
    {
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public decimal Price { get; set; }
        public bool Active { get; set; } = true;
    }

    [HttpPut]
    public async Task<IActionResult> Upsert([FromBody] UpsertProductRequest req)
    {
        var result = await _mediator.Send(new UpsertProductCommand(
            CompanyId,
            req.Sku,
            req.Name,
            req.Unit,
            req.Price,
            req.Active
        ));
        return ProcessResult(result);
    }
}

