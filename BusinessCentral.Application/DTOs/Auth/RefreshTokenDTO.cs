
using System.ComponentModel.DataAnnotations;


namespace BusinessCentral.Application.DTOs.Auth
{
    public class RefreshTokenRequestDTO
    {
        public string RefreshToken { get; set; } = null!; // Un GUID o string aleatorio largo
    }
}
