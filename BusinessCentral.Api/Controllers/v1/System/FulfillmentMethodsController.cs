using BusinessCentral.Api.Common;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.System;

[Authorize(Policy = "SystemRole")]
[Route("api/v1/system/config/fulfillment-methods")]
public sealed class FulfillmentMethodsController : ApiControllerBase
{
    private readonly IFulfillmentMethodRepository _repo;

    public FulfillmentMethodsController(IFulfillmentMethodRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool onlyActive = true)
    {
        var data = await _repo.ListMethodsAsync(onlyActive);
        return Ok(ApiResponse<object>.Success(data, "OK", 200));
    }

    [HttpGet("companies/{companyId:int}")]
    public async Task<IActionResult> ListCompany([FromRoute] int companyId, [FromQuery] bool onlyEnabled = true)
    {
        var data = await _repo.ListCompanyMethodsAsync(companyId, onlyEnabled);
        return Ok(ApiResponse<object>.Success(data, "OK", 200));
    }

    [HttpPut("companies/{companyId:int}/{methodCode}")]
    public async Task<IActionResult> SetCompany(
        [FromRoute] int companyId,
        [FromRoute] string methodCode,
        [FromQuery] bool enabled = true)
    {
        var ok = await _repo.SetCompanyMethodAsync(companyId, methodCode, enabled);
        if (!ok)
            return StatusCode(400, ApiResponse<object>.Failure("No se pudo actualizar el método.", 400));

        return Ok(ApiResponse<object>.Success(new { companyId, methodCode, enabled }, "OK", 200));
    }
}

