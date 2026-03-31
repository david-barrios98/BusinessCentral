using BusinessCentral.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BusinessCentral.Domain.Entities.Business
{
    [Table("Companies", Schema = "business")]
    public class Companies
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!; // Razón Social

        [MaxLength(200)]
        public string? TradeName { get; set; } // Nombre Comercial

        // --- Identificación Legal ---
        public int? DocumentTypeId { get; set; }

        [ForeignKey("DocumentTypeId")]
        public virtual DocumentType? DocumentType { get; set; } 

        [MaxLength(20)]
        public string? DocumentNumber { get; set; } // El número de NIT/RUT

        [MaxLength(5)]
        public string? VerificationDigit { get; set; } // Muy común en países como Colombia (DV)

        // --- Contacto y Web ---

        [MaxLength(150)]
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(10)]
        public string? Phone { get; set; }

        [MaxLength(255)]
        public string? Website { get; set; }

        [MaxLength(255)]
        public string? LogoUrl { get; set; }

        // --- Configuración Multi-tenant ---

        [Required]
        [MaxLength(50)]
        public string Subdomain { get; set; } = null!; // Ej: "miempresa.tuapp.com"

        public DateTime Create { get; set; }

        public DateTime Update { get; set; }

        public bool? Active { get; set; } = true;

    }
}
