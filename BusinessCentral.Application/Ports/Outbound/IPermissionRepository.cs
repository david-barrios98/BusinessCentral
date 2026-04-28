using BusinessCentral.Application.DTOs.Auth;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IPermissionRepository
{
    Task<List<PermissionDTO>> ListPermissionsAsync(bool onlyActive = true, string? moduleCode = null);
    Task<List<RolePermissionDTO>> ListDefaultPermissionsForNatureAsync(string natureCode);
}

