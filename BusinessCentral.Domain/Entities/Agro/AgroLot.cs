using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Agro;

[Table("AgroLot", Schema = "agro")]
public sealed class AgroLot
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Kind { get; set; } = "POULTRY"; // POULTRY/AQUA

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Name { get; set; }

    public DateTime StartDate { get; set; }

    public int InitialUnits { get; set; }
    public int CurrentUnits { get; set; }

    public decimal? InitialAvgWeightKg { get; set; } // útil para AQUA

    [MaxLength(20)]
    public string Status { get; set; } = "open"; // open/closed

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public int? CreatedByUserId { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }
}

