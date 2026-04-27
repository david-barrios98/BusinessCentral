using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Farm;

[Table("HarvestLot", Schema = "farm")]
public sealed class HarvestLot
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    public int? ZoneId { get; set; }

    [Required]
    public DateTime HarvestDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string ProductForm { get; set; } = string.Empty;

    // Opcional: referencia a catálogo (si lo usas). Mantengo ProductForm string por compatibilidad.
    public int? ProductFormTypeId { get; set; }

    [Required]
    public decimal QuantityKg { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(ZoneId))]
    public FarmZone? Zone { get; set; }

    [ForeignKey(nameof(ProductFormTypeId))]
    public CoffeeProductFormType? ProductFormType { get; set; }
}

