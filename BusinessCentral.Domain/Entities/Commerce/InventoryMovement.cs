using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Commerce;

[Table("InventoryMovement", Schema = "com")]
public sealed class InventoryMovement
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public int ProductId { get; set; }

    public long? VariantId { get; set; }

    // Ubicaciones opcionales: si no usas ubicaciones, quedan NULL y no afecta.
    public long? FromLocationId { get; set; }
    public long? ToLocationId { get; set; }

    public DateTime MoveDate { get; set; }

    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = string.Empty; // IN/OUT/ADJUST

    public decimal Quantity { get; set; }
    public string? ReferenceType { get; set; }
    public string? ReferenceId { get; set; }
    public string? Notes { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }

    [ForeignKey(nameof(VariantId))]
    public ProductVariant? Variant { get; set; }

    [ForeignKey(nameof(FromLocationId))]
    public BusinessCentral.Domain.Entities.Business.StorageLocation? FromLocation { get; set; }

    [ForeignKey(nameof(ToLocationId))]
    public BusinessCentral.Domain.Entities.Business.StorageLocation? ToLocation { get; set; }
}

