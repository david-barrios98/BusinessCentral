using System.Security.Claims;
using BusinessCentral.Api.Common;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Auth;

[Authorize]
[RequiresModule("AUTH")]
[Route("api/v1/secure/auth")]
public sealed class AuthBootstrapController : ApiControllerBase
{
    private readonly ICompanyModuleRepository _companyModules;
    private readonly IRolePermissionRepository _rolePermissions;

    public AuthBootstrapController(ICompanyModuleRepository companyModules, IRolePermissionRepository rolePermissions)
    {
        _companyModules = companyModules;
        _rolePermissions = rolePermissions;
    }

    [HttpGet("bootstrap")]
    public async Task<IActionResult> Bootstrap()
    {
        var userIdStr = User.FindFirstValue("userId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        var companyIdStr = User.FindFirstValue("companyId");
        var roleIdStr = User.FindFirstValue("roleId");

        _ = int.TryParse(userIdStr, out var userId);
        _ = int.TryParse(companyIdStr, out var companyId);
        _ = int.TryParse(roleIdStr, out var roleId);

        var isSystemRole = string.Equals(User.FindFirstValue("isSystemRole"), "true", StringComparison.OrdinalIgnoreCase);
        var isSuperUser = string.Equals(User.FindFirstValue("isSuperUser"), "true", StringComparison.OrdinalIgnoreCase);

        var modules = companyId > 0
            ? await _companyModules.ListCompanyModulesAsync(companyId)
            : new List<BusinessCentral.Application.DTOs.Config.CompanyModuleDTO>();

        var permissions = roleId > 0
            ? await _rolePermissions.ListRolePermissionsAsync(roleId)
            : new List<BusinessCentral.Application.DTOs.Auth.RolePermissionDTO>();

        return Ok(ApiResponse<object>.Success(new
        {
            userId,
            userName = User.FindFirstValue("userName"),
            companyId,
            companyName = User.FindFirstValue("companyName"),
            roleId,
            roleName = User.FindFirstValue("role"),
            isSystemRole,
            isSuperUser,
            modules,
            permissions
        }, "OK", 200));
    }
}

