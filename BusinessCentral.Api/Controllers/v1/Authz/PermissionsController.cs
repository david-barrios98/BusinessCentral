using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.Ports.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Authz;

[Authorize]
[RequiresModule("AUTH")]
[Route("api/v1/secure/auth/permissions")]
public sealed class PermissionsController : SecureCompanyControllerBase
{
    private readonly IPermissionRepository _permissions;

    public PermissionsController(IPermissionRepository permissions) => _permissions = permissions;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool onlyActive = true, [FromQuery] string? moduleCode = null)
    {
        var list = await _permissions.ListPermissionsAsync(onlyActive, moduleCode);
        return Ok(BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Success(list, "OK", 200));
    }

    [HttpGet("business-natures/{natureCode}/defaults")]
    public async Task<IActionResult> ListNatureDefaults([FromRoute] string natureCode)
    {
        var list = await _permissions.ListDefaultPermissionsForNatureAsync(natureCode);
        return Ok(BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Success(list, "OK", 200));
    }
}

