using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Manufacturing;

[Table("Recipe", Schema = "mfg")]
public sealed class Recipe
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public int OutputProductId { get; set; } // Producto vendido (plato/bebida)

    public long? OutputVariantId { get; set; }

    [Required]
    public decimal OutputQuantity { get; set; } = 1; // cantidad producida por receta

    [MaxLength(500)]
    public string? Notes { get; set; }

    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }
}

