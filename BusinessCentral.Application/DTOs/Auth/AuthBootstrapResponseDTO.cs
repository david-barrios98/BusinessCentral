using System.Text.Json.Serialization;
using BusinessCentral.Application.DTOs.Config;

namespace BusinessCentral.Application.DTOs.Auth;

public sealed class AuthBootstrapResponseDTO
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("userName")]
    public string? UserName { get; set; }

    [JsonPropertyName("companyId")]
    public int CompanyId { get; set; }

    [JsonPropertyName("companyName")]
    public string? CompanyName { get; set; }

    [JsonPropertyName("roleId")]
    public int RoleId { get; set; }

    [JsonPropertyName("roleName")]
    public string? RoleName { get; set; }

    [JsonPropertyName("isSystemRole")]
    public bool IsSystemRole { get; set; }

    [JsonPropertyName("isSuperUser")]
    public bool IsSuperUser { get; set; }

    [JsonPropertyName("modules")]
    public List<CompanyModuleDTO> Modules { get; set; } = new();

    [JsonPropertyName("permissions")]
    public List<RolePermissionDTO> Permissions { get; set; } = new();
}

