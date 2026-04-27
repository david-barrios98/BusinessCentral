using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Farm;

[Table("CoffeeProcessStep", Schema = "farm")]
public sealed class CoffeeProcessStep
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public long HarvestLotId { get; set; }

    [Required]
    public DateTime StepDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string StepType { get; set; } = string.Empty;

    public decimal? InputKg { get; set; }
    public decimal? OutputKg { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(HarvestLotId))]
    public HarvestLot? HarvestLot { get; set; }
}

