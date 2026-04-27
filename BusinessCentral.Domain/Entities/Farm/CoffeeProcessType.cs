using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Farm;

[Table("CoffeeProcessType", Schema = "farm")]
public sealed class CoffeeProcessType
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty; // PULPING/FERMENT/WASH/DRY/HULL/SORT/ROAST

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }
}

