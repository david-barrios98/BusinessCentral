using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Manufacturing;

[Table("ProductionBatch", Schema = "mfg")]
public sealed class ProductionBatch
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    public long? RecipeId { get; set; }

    [Required]
    public int OutputProductId { get; set; }

    public long? OutputVariantId { get; set; }

    public decimal QuantityProduced { get; set; }

    public long? ToLocationId { get; set; } // opcional

    public DateTime BatchDate { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "draft"; // draft/posted/void

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }
}

