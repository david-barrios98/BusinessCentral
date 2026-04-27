using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Commerce;

[Table("Product", Schema = "com")]
public sealed class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Sku { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Barcode { get; set; }

    [MaxLength(20)]
    public string? Unit { get; set; }

    // Precio de venta (base). Más adelante puedes manejar listas de precios.
    public decimal Price { get; set; }

    // Costo referencial (para margen / kardex futuro).
    public decimal? Cost { get; set; }

    // Impuestos (simple): código o etiqueta (ej: IVA_19). Mantiene flexibilidad.
    [MaxLength(50)]
    public string? TaxCode { get; set; }

    // Si false: no descuenta inventario (servicios, combos virtuales, etc.)
    public bool TrackInventory { get; set; } = true;

    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public int? CreatedByUserId { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }
}

