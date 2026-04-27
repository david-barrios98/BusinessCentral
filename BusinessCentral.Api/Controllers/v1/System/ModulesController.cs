using BusinessCentral.Api.Common;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.System;

[Authorize(Policy = "SystemRole")]
[Route("api/v1/system/config/modules")]
public sealed class ModulesController : ApiControllerBase
{
    private readonly ICompanyModuleRepository _companyModules;

    public ModulesController(ICompanyModuleRepository companyModules)
    {
        _companyModules = companyModules;
    }

    [HttpGet]
    public async Task<IActionResult> ListModules([FromQuery] bool onlyActive = true)
    {
        var data = await _companyModules.ListModulesAsync(onlyActive);
        return Ok(ApiResponse<object>.Success(data, "OK", 200));
    }

    [HttpGet("companies/{companyId:int}")]
    public async Task<IActionResult> ListCompanyModules([FromRoute] int companyId)
    {
        var data = await _companyModules.ListCompanyModulesAsync(companyId);
        return Ok(ApiResponse<object>.Success(data, "OK", 200));
    }

    [HttpPut("companies/{companyId:int}/{moduleCode}")]
    public async Task<IActionResult> SetCompanyModule(
        [FromRoute] int companyId,
        [FromRoute] string moduleCode,
        [FromQuery] bool enabled = true)
    {
        var ok = await _companyModules.SetCompanyModuleAsync(companyId, moduleCode, enabled);
        if (!ok)
            return StatusCode(400, ApiResponse<object>.Failure("No se pudo actualizar el módulo.", 400));

        return Ok(ApiResponse<object>.Success(new { companyId, moduleCode, enabled }, "OK", 200));
    }
}

