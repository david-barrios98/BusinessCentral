using BusinessCentral.Api.Common;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.System;

[Authorize(Policy = "SystemRole")]
[Route("api/v1/system/config/catalog/membership-plans")]
public sealed class MembershipPlansController : ApiControllerBase
{
    private readonly ICommonRepository _common;

    public MembershipPlansController(ICommonRepository common)
    {
        _common = common;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var data = await _common.ListMembershipPlansAsync();
        return Ok(ApiResponse<object>.Success(data, "OK", 200));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var data = await _common.GetMembershipPlanByIdAsync(id);
        if (data == null)
            return NotFound(ApiResponse<object>.Failure("Plan no encontrado.", 404));

        return Ok(ApiResponse<object>.Success(data, "OK", 200));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MembershipPlanRequest body)
    {
        var id = await _common.UpsertMembershipPlanAsync(null, body);
        return Ok(ApiResponse<object>.Success(new { id }, "OK", 200));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] MembershipPlanRequest body)
    {
        var newId = await _common.UpsertMembershipPlanAsync(id, body);
        return Ok(ApiResponse<object>.Success(new { id = newId }, "OK", 200));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await _common.DeleteMembershipPlanAsync(id);
        return Ok(ApiResponse<object>.Success(new { id, deleted = true }, "OK", 200));
    }
}

