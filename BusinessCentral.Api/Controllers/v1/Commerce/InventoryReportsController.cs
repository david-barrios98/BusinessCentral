using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.Feature.Commerce.Inventory.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Commerce;

[Authorize]
[RequiresModule("COMMERCE")]
[Route("api/v1/secure/commerce/reports/inventory")]
public sealed class InventoryReportsController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public InventoryReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("by-location")]
    public async Task<IActionResult> ByLocation([FromQuery] DateTime asOfUtc, [FromQuery] long? locationId = null)
    {
        var result = await _mediator.Send(new GetInventoryByLocationQuery(CompanyId, asOfUtc, locationId));
        return ProcessResult(result);
    }
}

