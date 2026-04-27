using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Commerce;

[Table("ProductAttributeOption", Schema = "com")]
public sealed class ProductAttributeOption
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public int AttributeId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty; // RED, 42, YAMAHA

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public int SortOrder { get; set; } = 0;

    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(AttributeId))]
    public ProductAttribute? Attribute { get; set; }
}

