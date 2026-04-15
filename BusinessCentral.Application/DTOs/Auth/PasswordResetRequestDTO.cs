using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BusinessCentral.Application.DTOs.Auth
{
    public class PasswordResetRequestDTO
    {
        [JsonPropertyName("email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [JsonPropertyName("companyId")]
        [Required]
        public int CompanyId { get; set; }
    }

    public class PasswordResetConfirmDTO
    {
        [JsonPropertyName("token")]
        [Required]
        public string Token { get; set; } = null!;

        [JsonPropertyName("newPassword")]
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = null!;
    }
}