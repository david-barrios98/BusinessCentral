using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Commerce;

[Table("ProductCategory", Schema = "com")]
public sealed class ProductCategory
{
    public int CompanyId { get; set; }
    public int ProductId { get; set; }
    public int CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }
}

