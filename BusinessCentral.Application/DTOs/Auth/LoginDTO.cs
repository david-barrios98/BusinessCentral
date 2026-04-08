using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BusinessCentral.Application.DTOs.Auth;

/// <summary>
/// DTO de entrada: Solo contiene credenciales necesarias
/// </summary>
public class LoginRequestDTO
{
    [JsonPropertyName("username")]
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    public string UserName { get; set; } = null!;

    [JsonPropertyName("password")]
    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(2, ErrorMessage = "Mínimo 8 caracteres")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [JsonPropertyName("companyId")]
    [Required(ErrorMessage = "La compañia es requerida")]
    public string CompanyId { get; set; } = null!;
}

/// <summary>
/// DTO de salida: NO incluye contraseña ni datos sensibles
/// </summary>
public class LoginResponseDTO
{
    [JsonPropertyName("user_id")]
    public int user_id { get; set; }

    [JsonPropertyName("username")]
    public string username { get; set; } = null!;

    [JsonPropertyName("password")]
    public string password { get; set; } = null!;

    [JsonPropertyName("email")]
    public string? email { get; set; }

    [JsonPropertyName("first_name")]
    public string? first_name { get; set; }

    [JsonPropertyName("last_name")]
    public string? last_name { get; set; }

    [JsonPropertyName("access_token")]
    public string access_token { get; set; } = null!;

    [JsonPropertyName("token_type")]
    public string token_type { get; set; } = "Bearer";

    [JsonPropertyName("expires_in")]
    public int expires_in { get; set; }

    [JsonPropertyName("issued_at")]
    public DateTime issued_at { get; set; }
}
