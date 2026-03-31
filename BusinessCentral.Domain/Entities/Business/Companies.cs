using BusinessCentral.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BusinessCentral.Domain.Entities.Business
{
    [Table("companies", Schema = "business")]
    public class Companies
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("name")]
        public string Name { get; set; } = null!; // Razón Social

        [MaxLength(200)]
        [Column("trade_name")]
        public string? TradeName { get; set; } // Nombre Comercial

        // --- Identificación Legal ---

        [Required]
        [Column("document_type_id")]
        public int DocumentTypeId { get; set; }

        [ForeignKey("DocumentTypeId")]
        public virtual DocumentType DocumentType { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [Column("document_number")]
        public string DocumentNumber { get; set; } = null!; // El número de NIT/RUT

        [MaxLength(5)]
        [Column("verification_digit")]
        public string? VerificationDigit { get; set; } // Muy común en países como Colombia (DV)

        // --- Contacto y Web ---

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        [Column("email")]
        public string Email { get; set; } = null!;

        [MaxLength(20)]
        [Column("phone")]
        public string? Phone { get; set; }

        [MaxLength(255)]
        [Column("website")]
        public string? Website { get; set; }

        [MaxLength(255)]
        [Column("logo_url")]
        public string? LogoUrl { get; set; }

        // --- Configuración Multi-tenant ---

        [Required]
        [MaxLength(50)]
        [Column("subdomain")]
        public string Subdomain { get; set; } = null!; // Ej: "miempresa.tuapp.com"

        [Column("create")]
        public DateTime Create { get; set; }

        [Column("update")]
        public DateTime Update { get; set; }

        [Column("active")]
        public bool? Active { get; set; } = true;

    }
}
