using BusinessCentral.Api.Common;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.DTOs.Config;
using BusinessCentral.Application.Ports.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.System;

[Authorize(Policy = "SystemRole")]
[Route("api/v1/system/config/onboarding")]
public sealed class CompanyOnboardingController : ApiControllerBase
{
    private readonly ICompanyOnboardingRepository _onboarding;
    private readonly IHashPasswordService _hash;

    public CompanyOnboardingController(ICompanyOnboardingRepository onboarding, IHashPasswordService hash)
    {
        _onboarding = onboarding;
        _hash = hash;
    }

    [HttpGet("business-natures")]
    public async Task<IActionResult> ListBusinessNatures([FromQuery] bool onlyActive = true)
    {
        var data = await _onboarding.ListBusinessNaturesAsync(onlyActive);
        return Ok(ApiResponse<object>.Success(data, "OK", 200));
    }

    [HttpGet("business-natures/{code}/modules")]
    public async Task<IActionResult> ListBusinessNatureModules([FromRoute] string code)
    {
        var data = await _onboarding.ListBusinessNatureModulesAsync(code);
        return Ok(ApiResponse<object>.Success(data, "OK", 200));
    }

    [HttpGet("companies/{companyId:int}/business-natures")]
    public async Task<IActionResult> ListCompanyBusinessNatures([FromRoute] int companyId)
    {
        var data = await _onboarding.ListCompanyBusinessNaturesAsync(companyId);
        return Ok(ApiResponse<object>.Success(data, "OK", 200));
    }

    [HttpPut("companies/{companyId:int}/business-natures/{natureCode}")]
    public async Task<IActionResult> SetCompanyBusinessNature(
        [FromRoute] int companyId,
        [FromRoute] string natureCode,
        [FromQuery] bool enabled = true,
        [FromQuery] bool primary = false)
    {
        var ok = await _onboarding.SetCompanyBusinessNatureAsync(companyId, natureCode, primary, enabled);
        if (!ok)
            return StatusCode(400, ApiResponse<object>.Failure("No se pudo actualizar la naturaleza.", 400));

        return Ok(ApiResponse<object>.Success(new { companyId, natureCode, enabled, primary }, "OK", 200));
    }

    [HttpPost("companies")]
    public async Task<IActionResult> OnboardCompany([FromBody] OnboardCompanyRequestDTO request)
    {
        var passwordHash = _hash.Hash(request.OwnerPassword);
        var result = await _onboarding.OnboardCompanyAsync(request, passwordHash);
        if (!result.Success)
            return StatusCode(400, ApiResponse<object>.Failure("No se pudo crear la compañía.", 400));

        return Ok(ApiResponse<object>.Success(result, "OK", 200));
    }
}

