using BusinessCentral.Api.Common;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.DTOs.Config;
using BusinessCentral.Application.Ports.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.System;

[Authorize(Policy = "SystemRole")]
[Route("api/v1/system/config/catalog/business-natures")]
public sealed class BusinessNaturesController : ApiControllerBase
{
    private readonly IBusinessNatureRepository _repo;

    public BusinessNaturesController(IBusinessNatureRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool onlyActive = true)
    {
        var data = await _repo.ListAsync(onlyActive);
        return Ok(ApiResponse<object>.Success(data, "OK", 200));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var data = await _repo.GetByIdAsync(id);
        if (data == null)
            return NotFound(ApiResponse<object>.Failure("Naturaleza no encontrada.", 404));

        return Ok(ApiResponse<object>.Success(data, "OK", 200));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertBusinessNatureRequestDTO body)
    {
        body.Id = null;
        var id = await _repo.UpsertAsync(body);
        return Ok(ApiResponse<object>.Success(new { id }, "OK", 200));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpsertBusinessNatureRequestDTO body)
    {
        body.Id = id;
        var newId = await _repo.UpsertAsync(body);
        return Ok(ApiResponse<object>.Success(new { id = newId }, "OK", 200));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await _repo.DeleteAsync(id);
        return Ok(ApiResponse<object>.Success(new { id, deleted = true }, "OK", 200));
    }
}

