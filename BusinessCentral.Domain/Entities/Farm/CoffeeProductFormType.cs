using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Farm;

[Table("CoffeeProductFormType", Schema = "farm")]
public sealed class CoffeeProductFormType
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty; // CHERRY/PULP/DRY_PARCHMENT/GREEN/ROASTED

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }
}

