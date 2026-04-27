using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Agro;

[Table("AgroHarvest", Schema = "agro")]
public sealed class AgroHarvest
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public long LotId { get; set; }

    public DateTime HarvestDate { get; set; }

    [Required]
    public int OutputProductId { get; set; } // pollo/pescado vendido como producto

    public long? OutputVariantId { get; set; }

    public int Units { get; set; }
    public decimal? TotalWeightKg { get; set; }

    public long? ToLocationId { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }
}

