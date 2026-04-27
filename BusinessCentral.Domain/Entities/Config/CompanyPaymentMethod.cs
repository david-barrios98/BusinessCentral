using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Config;

[Table("CompanyPaymentMethod", Schema = "config")]
public sealed class CompanyPaymentMethod
{
    public int CompanyId { get; set; }
    public int PaymentMethodId { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Companies? Company { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
}

