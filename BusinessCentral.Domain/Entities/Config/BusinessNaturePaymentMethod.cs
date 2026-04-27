using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Config;

[Table("BusinessNaturePaymentMethod", Schema = "config")]
public sealed class BusinessNaturePaymentMethod
{
    public int BusinessNatureId { get; set; }
    public int PaymentMethodId { get; set; }
    public bool IsDefaultEnabled { get; set; } = true;
    public int SortOrder { get; set; } = 0;

    public BusinessNature? BusinessNature { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
}

