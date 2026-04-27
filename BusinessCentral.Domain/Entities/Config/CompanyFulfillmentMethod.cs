using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Config;

[Table("CompanyFulfillmentMethod", Schema = "config")]
public sealed class CompanyFulfillmentMethod
{
    public int CompanyId { get; set; }
    public int FulfillmentMethodId { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Companies? Company { get; set; }
    public FulfillmentMethod? FulfillmentMethod { get; set; }
}

