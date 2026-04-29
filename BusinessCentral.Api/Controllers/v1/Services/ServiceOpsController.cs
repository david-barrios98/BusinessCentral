using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.DTOs.Services;
using BusinessCentral.Application.Ports.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Services;

[Authorize]
[RequiresModule("SERVICES")]
[Route("api/v1/secure/services/ops")]
public sealed class ServiceOpsController : SecureCompanyControllerBase
{
    private readonly IServicesRepository _services;

    public ServiceOpsController(IServicesRepository services)
    {
        _services = services;
    }

    // --- Settings ---
    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings()
    {
        var dto = await _services.GetCompanySettingsAsync(CompanyId);
        return Ok(BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Success(dto, "OK", 200));
    }

    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateServiceCompanySettingsRequest body)
    {
        var ok = await _services.UpdateCompanySettingsAsync(CompanyId, body);
        if (!ok)
            return StatusCode(400, BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Failure("No se pudo actualizar la configuración.", 400));

        return Ok(BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Success(new { updated = true }, "OK", 200));
    }

    // --- Coverage ---
    [HttpGet("coverage-areas")]
    public async Task<IActionResult> ListCoverage([FromQuery] bool onlyActive = true)
    {
        var list = await _services.ListCoverageAreasAsync(CompanyId, onlyActive);
        return Ok(BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Success(list, "OK", 200));
    }

    [HttpPost("coverage-areas")]
    public async Task<IActionResult> CreateCoverage([FromBody] UpsertServiceCoverageAreaRequest body)
    {
        body.Id = null;
        var id = await _services.UpsertCoverageAreaAsync(CompanyId, body);
        return Ok(BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Success(new { id }, "OK", 200));
    }

    [HttpPut("coverage-areas/{id:int}")]
    public async Task<IActionResult> UpdateCoverage([FromRoute] int id, [FromBody] UpsertServiceCoverageAreaRequest body)
    {
        body.Id = id;
        var newId = await _services.UpsertCoverageAreaAsync(CompanyId, body);
        return Ok(BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Success(new { id = newId }, "OK", 200));
    }

    [HttpDelete("coverage-areas/{id:int}")]
    public async Task<IActionResult> DeleteCoverage([FromRoute] int id)
    {
        await _services.DeleteCoverageAreaAsync(CompanyId, id);
        return Ok(BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Success(new { id, deleted = true }, "OK", 200));
    }

    // --- Shifts ---
    [HttpGet("shifts")]
    public async Task<IActionResult> ListShifts([FromQuery] bool onlyActive = true)
    {
        var list = await _services.ListShiftTemplatesAsync(CompanyId, onlyActive);
        return Ok(BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Success(list, "OK", 200));
    }

    [HttpPost("shifts")]
    public async Task<IActionResult> CreateShift([FromBody] UpsertServiceShiftTemplateRequest body)
    {
        body.Id = null;
        var id = await _services.UpsertShiftTemplateAsync(CompanyId, body);
        return Ok(BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Success(new { id }, "OK", 200));
    }

    [HttpPut("shifts/{id:int}")]
    public async Task<IActionResult> UpdateShift([FromRoute] int id, [FromBody] UpsertServiceShiftTemplateRequest body)
    {
        body.Id = id;
        var newId = await _services.UpsertShiftTemplateAsync(CompanyId, body);
        return Ok(BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Success(new { id = newId }, "OK", 200));
    }

    [HttpDelete("shifts/{id:int}")]
    public async Task<IActionResult> DeleteShift([FromRoute] int id)
    {
        await _services.DeleteShiftTemplateAsync(CompanyId, id);
        return Ok(BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Success(new { id, deleted = true }, "OK", 200));
    }
}

