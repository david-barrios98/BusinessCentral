using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.Feature.Business.StorageLocations.Commands;
using BusinessCentral.Application.Feature.Business.StorageLocations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Business;

[Authorize]
[RequiresModule("COMMERCE")] // ubicaciones se usan típicamente con inventario; si no, no bloquea el resto.
[Route("api/v1/secure/business/storage-locations")]
public sealed class StorageLocationsController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public StorageLocationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? facilityId = null, [FromQuery] bool onlyActive = true)
    {
        var result = await _mediator.Send(new ListStorageLocationsQuery(CompanyId, facilityId, onlyActive));
        return ProcessResult(result);
    }

    public sealed class UpsertRequest
    {
        public long? Id { get; set; }
        public int? FacilityId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "WAREHOUSE";
        public long? ParentLocationId { get; set; }
        public string? Notes { get; set; }
        public bool Active { get; set; } = true;
    }

    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] UpsertRequest req)
    {
        var result = await _mediator.Send(new UpsertStorageLocationCommand(
            CompanyId,
            req.Id,
            req.FacilityId,
            req.Code,
            req.Name,
            req.Type,
            req.ParentLocationId,
            req.Notes,
            req.Active
        ));
        return ProcessResult(result);
    }
}

