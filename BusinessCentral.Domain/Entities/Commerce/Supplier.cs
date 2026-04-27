using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Commerce;

[Table("Supplier", Schema = "com")]
public sealed class Supplier
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? DocumentNumber { get; set; } // NIT/CC

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(150)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public int? CreatedByUserId { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }
}

