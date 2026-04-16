using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BusinessCentral.Application.DTOs.Auth
{
    public class CreateUserDTO
    {
        [Required] public int CompanyId { get; set; }
        [Required] public int DocumentTypeId { get; set; }
        [JsonPropertyName("documentNumber")] public string? DocumentNumber { get; set; }
        [Required] public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        [EmailAddress] public string? Email { get; set; }
        [Required] public string Phone { get; set; } = null!;
        [Required] public string Password { get; set; } = null!;
        public string AuthProvider { get; set; } = "Local";
        public string? ExternalId { get; set; }
        [Required] public int RoleId { get; set; }
    }

    public class UpdateUserDTO
    {
        [Required] public int UserId { get; set; }
        [Required] public int DocumentTypeId { get; set; }
        public string? DocumentNumber { get; set; }
        [Required] public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        [EmailAddress] public string? Email { get; set; }
        [Required] public string Phone { get; set; } = null!;
        public string? Password { get; set; } // null = no change
        public string AuthProvider { get; set; } = "Local";
        public string? ExternalId { get; set; }
        [Required] public int RoleId { get; set; }
        public bool Active { get; set; } = true;
    }

    public class UserResponseDTO
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? DocumentNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string Phone { get; set; } = null!;
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}