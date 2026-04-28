namespace BusinessCentral.Application.DTOs.Auth;

public sealed class PermissionDTO
{
    public int PermissionId { get; set; }
    public int ModuleId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string PermissionCode { get; set; } = string.Empty;
    public string PermissionName { get; set; } = string.Empty;
    public bool Active { get; set; }
}

