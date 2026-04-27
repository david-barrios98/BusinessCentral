using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.Feature.Services.Catalog;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Services;

[Authorize]
[RequiresModule("SERVICES")]
[Route("api/v1/secure/services/catalog")]
public sealed class ServiceCatalogController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public ServiceCatalogController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool onlyActive = true)
    {
        var result = await _mediator.Send(new ListServicesQuery(CompanyId, onlyActive));
        return ProcessResult(result);
    }

    public sealed class UpsertServiceRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public bool Active { get; set; } = true;
    }

    [HttpPut]
    public async Task<IActionResult> Upsert([FromBody] UpsertServiceRequest req)
    {
        var result = await _mediator.Send(new UpsertServiceCommand(CompanyId, req.Code, req.Name, req.BasePrice, req.Active));
        return ProcessResult(result);
    }
}

