using BusinessCentral.Api.Common;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.DTOs.Config;
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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var data = await _companyModules.GetModuleByIdAsync(id);
        if (data == null)
            return NotFound(ApiResponse<object>.Failure("Módulo no encontrado.", 404));

        return Ok(ApiResponse<object>.Success(data, "OK", 200));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertModuleRequestDTO body)
    {
        body.Id = null;
        var id = await _companyModules.UpsertModuleAsync(body);
        return Ok(ApiResponse<object>.Success(new { id }, "OK", 200));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpsertModuleRequestDTO body)
    {
        body.Id = id;
        var newId = await _companyModules.UpsertModuleAsync(body);
        return Ok(ApiResponse<object>.Success(new { id = newId }, "OK", 200));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await _companyModules.DeleteModuleAsync(id);
        return Ok(ApiResponse<object>.Success(new { id, deleted = true }, "OK", 200));
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

