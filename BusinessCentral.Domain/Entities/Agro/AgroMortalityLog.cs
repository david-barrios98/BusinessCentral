using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Agro;

[Table("AgroMortalityLog", Schema = "agro")]
public sealed class AgroMortalityLog
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public long LotId { get; set; }

    public DateTime MortalityDate { get; set; }

    public int Units { get; set; }

    public decimal? AvgWeightKg { get; set; } // útil para AQUA

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }
}

