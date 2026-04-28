using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.Ports.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Authz;

[Authorize]
[RequiresModule("AUTH")]
[Route("api/v1/secure/auth/roles/{roleId:int}/permissions")]
public sealed class RolePermissionsController : SecureCompanyControllerBase
{
    private readonly IRolePermissionRepository _rolePermissions;

    public RolePermissionsController(IRolePermissionRepository rolePermissions) => _rolePermissions = rolePermissions;

    [HttpGet]
    public async Task<IActionResult> List([FromRoute] int roleId)
    {
        var list = await _rolePermissions.ListRolePermissionsAsync(roleId);
        return Ok(BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Success(list, "OK", 200));
    }

    public sealed class SetPermissionRequest
    {
        public int PermissionId { get; set; }
        public bool Enabled { get; set; } = true;
    }

    [HttpPut]
    public async Task<IActionResult> Set([FromRoute] int roleId, [FromBody] SetPermissionRequest req)
    {
        var ok = await _rolePermissions.SetRolePermissionAsync(roleId, req.PermissionId, req.Enabled);
        if (!ok)
            return StatusCode(400, BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Failure("No se pudo actualizar el permiso.", 400));

        return Ok(BusinessCentral.Application.DTOs.Common.ApiResponse<object>.Success(new
        {
            roleId,
            req.PermissionId,
            req.Enabled
        }, "OK", 200));
    }
}

