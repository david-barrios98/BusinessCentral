using BusinessCentral.Domain.Entities.Common;
using BusinessCentral.Domain.Entities.Config;
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

        [MaxLength(200)]
        public string? Name { get; set; } // Razón Social

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

        [MaxLength(50)]
        public string? Subdomain { get; set; } // Ej: "miempresa.tuapp.com"

        // --- Naturaleza del negocio (plantilla de módulos + defaults) ---
        public int? BusinessNatureId { get; set; }

        [ForeignKey(nameof(BusinessNatureId))]
        public virtual BusinessNature? BusinessNature { get; set; }

        public DateTime Create { get; set; }

        public DateTime Update { get; set; }

        public bool? Active { get; set; } = true;

    }
}
