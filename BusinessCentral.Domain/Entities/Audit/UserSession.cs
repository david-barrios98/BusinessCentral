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
    [Table("user_sessions", Schema = "audit")]
    public class UserSession
    {
        [Key]
        [Column("id")]
        public long Id { get; set; } // Usamos long por el alto volumen de registros

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UsersInfo User { get; set; } = null!;

        [Required]
        [Column("company_id")]
        public int CompanyId { get; set; } // Multi-tenant: saber a qué empresa entró

        [Required]
        [MaxLength(50)]
        [Column("platform")]
        public string Platform { get; set; } = null!; // "Web", "iOS", "Android", "Desktop"

        [MaxLength(255)]
        [Column("device_fingerprint")]
        public string? DeviceFingerprint { get; set; } // ID único del móvil o hash del navegador

        [MaxLength(45)]
        [Column("ip_address")]
        public string? IpAddress { get; set; }

        [MaxLength(500)]
        [Column("user_agent")]
        public string? UserAgent { get; set; } // Detalle del navegador/OS

        [Required]
        [Column("login_at")]
        public DateTime LoginAt { get; set; } = DateTime.UtcNow;

        [Column("logout_at")]
        public DateTime? LogoutAt { get; set; }

        [Required]
        [Column("is_success")]
        public bool IsSuccess { get; set; } = true;

        [MaxLength(100)]
        [Column("failure_reason")]
        public string? FailureReason { get; set; } // Ej: "Invalid Password", "Locked Account"
    }
}
