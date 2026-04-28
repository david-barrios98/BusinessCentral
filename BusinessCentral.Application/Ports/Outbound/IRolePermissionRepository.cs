using BusinessCentral.Application.DTOs.Auth;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IRolePermissionRepository
{
    Task<List<RolePermissionDTO>> ListRolePermissionsAsync(int roleId);
    Task<bool> SetRolePermissionAsync(int roleId, int permissionId, bool enabled);
}

