using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Commerce;

[Table("ProductVariant", Schema = "com")]
public sealed class ProductVariant
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public int ProductId { get; set; } // producto base

    [Required]
    [MaxLength(80)]
    public string Sku { get; set; } = string.Empty; // SKU específico por variante

    [MaxLength(50)]
    public string? Barcode { get; set; }

    [MaxLength(200)]
    public string? VariantName { get; set; } // Ej: "42 / Negro / Yamaha"

    public decimal? PriceOverride { get; set; } // opcional
    public decimal? CostOverride { get; set; } // opcional

    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public int? CreatedByUserId { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }
}

