using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Business;

[Table("StorageLocation", Schema = "business")]
public sealed class StorageLocation
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    // Opcional: para soportar multi-sede. Si es null, es global a la compañía.
    public int? FacilityId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty; // Ej: W1, W1-Z1, W1-SHELF-J

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty; // Ej: Bodega principal, Estante J, Vitrina 1

    [Required]
    [MaxLength(30)]
    public string Type { get; set; } = "WAREHOUSE"; // WAREHOUSE/ZONE/SHELF/SHOWCASE/BIN/AREA

    public long? ParentLocationId { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public int? CreatedByUserId { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(FacilityId))]
    public Facility? Facility { get; set; }

    [ForeignKey(nameof(ParentLocationId))]
    public StorageLocation? Parent { get; set; }
}

