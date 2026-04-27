using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.Feature.Commerce.Suppliers.Commands;
using BusinessCentral.Application.Feature.Commerce.Suppliers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Commerce;

[Authorize]
[RequiresModule("COMMERCE")]
[Route("api/v1/secure/commerce/suppliers")]
public sealed class SuppliersController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public SuppliersController(IMediator mediator)
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
        var result = await _mediator.Send(new ListSuppliersQuery(CompanyId, onlyActive, page, pageSize, q));
        return ProcessResult(result);
    }

    public sealed class UpsertRequest
    {
        public long? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? DocumentNumber { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Notes { get; set; }
        public bool Active { get; set; } = true;
    }

    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] UpsertRequest req)
    {
        var result = await _mediator.Send(new UpsertSupplierCommand(
            CompanyId,
            req.Id,
            req.Name,
            req.DocumentNumber,
            req.Phone,
            req.Email,
            req.Notes,
            req.Active,
            UserId == 0 ? null : UserId
        ));
        return ProcessResult(result);
    }
}

