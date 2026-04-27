using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Finance;

[Table("TaxConcept", Schema = "fin")]
public sealed class TaxConcept
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty; // IVA_19, ICA, RETEFUENTE_2_5, etc

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? RegulatoryEntity { get; set; } // DIAN / Municipio / etc

    public decimal? Rate { get; set; } // 0.19, 0.025, etc

    [MaxLength(500)]
    public string? Notes { get; set; }

    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

