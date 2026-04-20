using BusinessCentral.Domain.Entities.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BusinessCentral.Domain.Entities.Audit
{
    [Table("UserSession", Schema = "audit")]
    public class UserSession
    {
        [Key]
        public long Id { get; set; } // Usamos long por el alto volumen de registros

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UsersInfo User { get; set; } = null!;
        public string? LoginField { get; set; } = null; // Para auditoría, aunque ya esté en UsersInfo

        [Required]
        public int CompanyId { get; set; } // Multi-tenant: saber a qué empresa entró

        [Required]
        [MaxLength(50)]
        public string Platform { get; set; } = null!; // "Web", "iOS", "Android", "Desktop"

        [MaxLength(255)]
        public string? DeviceFingerprint { get; set; } // ID único del móvil o hash del navegador

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; } // Detalle del navegador/OS

        [Required]
        public DateTime LoginAt { get; set; } = DateTime.UtcNow;

        public DateTime? LogoutAt { get; set; }

        [Required]
        public bool IsSuccess { get; set; } = true;

        [MaxLength(100)]
        public string? FailureReason { get; set; } // Ej: "Invalid Password", "Locked Account"

        // Propiedad calculada para saber si la sesión sigue activa
        [NotMapped]
        public bool IsActive => LogoutAt == null && IsSuccess;
    }
}
