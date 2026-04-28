namespace BusinessCentral.Application.DTOs.Auth;

public sealed class RolePermissionDTO
{
    public int PermissionId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string PermissionCode { get; set; } = string.Empty;
    public string PermissionName { get; set; } = string.Empty;
}

