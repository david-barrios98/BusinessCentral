using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Manufacturing;

[Table("RecipeItem", Schema = "mfg")]
public sealed class RecipeItem
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public long RecipeId { get; set; }

    [Required]
    public int InputProductId { get; set; } // Insumo

    public long? InputVariantId { get; set; }

    [Required]
    public decimal Quantity { get; set; } // por OutputQuantity

    [MaxLength(20)]
    public string? Unit { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }
}

