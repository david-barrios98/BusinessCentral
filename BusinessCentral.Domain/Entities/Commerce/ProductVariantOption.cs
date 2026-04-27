using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Commerce;

[Table("ProductVariantOption", Schema = "com")]
public sealed class ProductVariantOption
{
    public int CompanyId { get; set; }
    public long VariantId { get; set; }
    public int AttributeId { get; set; }
    public int OptionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Companies? Company { get; set; }
    public ProductVariant? Variant { get; set; }
    public ProductAttribute? Attribute { get; set; }
    public ProductAttributeOption? Option { get; set; }
}

