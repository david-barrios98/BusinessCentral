using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Config;

[Table("BusinessNatureFulfillmentMethod", Schema = "config")]
public sealed class BusinessNatureFulfillmentMethod
{
    public int BusinessNatureId { get; set; }
    public int FulfillmentMethodId { get; set; }

    public bool IsDefaultEnabled { get; set; } = true;
    public int SortOrder { get; set; } = 0;

    public BusinessNature? BusinessNature { get; set; }
    public FulfillmentMethod? FulfillmentMethod { get; set; }
}

