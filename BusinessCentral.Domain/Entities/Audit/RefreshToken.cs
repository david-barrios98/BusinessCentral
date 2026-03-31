using BusinessCentral.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessCentral.Domain.Entities.Audit
{
    [Table("RefreshToken", Schema = "audit")]
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UsersInfo User { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Token { get; set; } = null!; // Un GUID o string aleatorio largo

        [Required]
        public DateTime ExpiresAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RevokedAt { get; set; } // Si el usuario hace Logout, se marca aquí

        public string? ReplacedByToken { get; set; } // Para auditoría de rotación de tokens

        public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;
    }
}
