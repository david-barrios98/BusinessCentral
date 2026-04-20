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
        public long Id { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UsersInfo User { get; set; } = null!;
        public string? LoginField { get; set; } = null;

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

        /// <summary>
        /// Guardamos companyId aquí para auditoría (aunque exista FK en UsersInfo).
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        /// Opcional: JTI del access token (si quieres trazar tokens por jti).
        /// </summary>
        [MaxLength(50)]
        public string? JwtId { get; set; }

        /// <summary>
        /// Fecha de expiración del access token relacionado (opcional).
        /// </summary>
        public DateTime? AccessTokenExpiresAt { get; set; }

        public bool? Active { get; set; } = null;
    }
}
