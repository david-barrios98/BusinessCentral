using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.Feature.Farm.Zones;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Farm;

[Authorize]
[RequiresModule("FARM")]
[Route("api/v1/secure/farm/zones")]
public sealed class ZonesController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public ZonesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool onlyActive = true)
    {
        var result = await _mediator.Send(new ListZonesQuery(CompanyId, onlyActive));
        return ProcessResult(result);
    }

    public sealed class UpsertZoneRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool Active { get; set; } = true;
    }

    [HttpPut]
    public async Task<IActionResult> Upsert([FromBody] UpsertZoneRequest req)
    {
        var result = await _mediator.Send(new UpsertZoneCommand(CompanyId, req.Code, req.Name, req.Active));
        return ProcessResult(result);
    }
}

