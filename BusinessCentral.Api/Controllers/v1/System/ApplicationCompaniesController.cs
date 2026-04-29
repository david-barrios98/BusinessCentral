using BusinessCentral.Api.Common;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.DTOs.Config;
using BusinessCentral.Application.Ports.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.System;

/// <summary>
/// Configura por compañía qué campo de login usa cada código de aplicación (APK, instalador, etc.).
/// </summary>
[Authorize(Policy = "SystemRole")]
[Route("api/v1/system/config/companies/{companyId:int}/application-companies")]
public sealed class ApplicationCompaniesController : ApiControllerBase
{
    private readonly IApplicationCompanyRepository _repo;

    public ApplicationCompaniesController(IApplicationCompanyRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromRoute] int companyId)
    {
        var data = await _repo.ListByCompanyAsync(companyId);
        return Ok(ApiResponse<object>.Success(data, "OK", 200));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromRoute] int companyId,
        [FromBody] UpsertApplicationCompanyRequestDTO body)
    {
        body.Id = null;
        var id = await _repo.UpsertAsync(companyId, body);
        return Ok(ApiResponse<object>.Success(new { id }, "OK", 200));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        [FromRoute] int companyId,
        [FromRoute] int id,
        [FromBody] UpsertApplicationCompanyRequestDTO body)
    {
        body.Id = id;
        var newId = await _repo.UpsertAsync(companyId, body);
        return Ok(ApiResponse<object>.Success(new { id = newId }, "OK", 200));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int companyId, [FromRoute] int id)
    {
        await _repo.DeleteAsync(companyId, id);
        return Ok(ApiResponse<object>.Success(new { companyId, id, deleted = true }, "OK", 200));
    }
}
