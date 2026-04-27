using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.DTOs.Farm;
using BusinessCentral.Application.Feature.Farm.HarvestLots;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Farm;

[Authorize]
[RequiresModule("FARM")]
[Route("api/v1/secure/farm/harvest-lots")]
public sealed class HarvestLotsController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public HarvestLotsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null, [FromQuery] int? zoneId = null)
    {
        var result = await _mediator.Send(new ListHarvestLotsQuery(CompanyId, from, to, zoneId));
        return ProcessResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] HarvestLotDTO dto)
    {
        dto.CompanyId = CompanyId;
        var result = await _mediator.Send(new CreateHarvestLotCommand(CompanyId, dto));
        return ProcessResult(result);
    }
}

