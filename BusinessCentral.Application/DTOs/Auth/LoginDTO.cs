using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BusinessCentral.Application.DTOs.Auth;

/// <summary>
/// DTO de entrada: Solo contiene credenciales necesarias
/// </summary>
public class LoginRequestDTO
{
    [JsonPropertyName("UserName")]
    [Required(ErrorMessage = "El UserName de usuario es requerido")]
    public string UserName { get; set; } = null!;

    [JsonPropertyName("Password")]
    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(2, ErrorMessage = "La contraseña requiere mínimo 2 caracteres")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [JsonPropertyName("companyId")]
    [Required(ErrorMessage = "La compañia es requerida")]
    [MinLength(1, ErrorMessage = "El companyId requiere mínimo 1 caracteres")]
    public string CompanyId { get; set; } = null!;
}

public class LogoutRequestDTO
{
    public int? UserId { get; set; } = null;
    public int? CompanyId { get; set; } = null;
    public string? RefreshToken { get; set; } = null;
    public long? SessionId { get; set; } = null;
}

/// <summary>
    /// DTO de salida: NO incluye contraseña ni datos sensibles
    /// </summary>
public class LoginResponseDTO
{
    [JsonPropertyName("userSessionId")]
    public long UserSessionId { get; set; }

    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("userName")]
    public string? UserName { get; set; }

    [JsonPropertyName("loginField")]
    public string? LoginField { get; set; } = null;

    [JsonPropertyName("documentNumber")]
    public string? DocumentNumber { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("companyId")]
    public int CompanyId { get; set; }

    [JsonPropertyName("companyName")]
    public string? CompanyName { get; set; }

    // --- Rol / autorización ---
    [JsonPropertyName("roleId")]
    public int RoleId { get; set; }

    [JsonPropertyName("roleName")]
    public string? RoleName { get; set; }

    [JsonPropertyName("isSystemRole")]
    public bool IsSystemRole { get; set; }

    [JsonPropertyName("isSuperUser")]
    public bool IsSuperUser { get; set; }

    [JsonPropertyName("permissions")]
    public List<RolePermissionDTO> Permissions { get; set; } = new();

    [JsonPropertyName("modules")]
    public List<BusinessCentral.Application.DTOs.Config.CompanyModuleDTO> Modules { get; set; } = new();

    // --- Datos de Seguridad (JWT) ---
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = null!;

    [JsonPropertyName("refreshToken")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RefreshToken { get; set; }

    [JsonPropertyName("tokenType")]
    public string TokenType { get; set; } = "Bearer";

    [JsonPropertyName("expiresIn")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("issuedAt")]
    public DateTime IssuedAt { get; set; }

    // Nota: Es mejor no devolver el Password en el DTO de respuesta por seguridad, 
    // pero se incluye si tu lógica de aplicación lo requiere para validaciones internas.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("password")]
    public string? Password { get; set; }
}
