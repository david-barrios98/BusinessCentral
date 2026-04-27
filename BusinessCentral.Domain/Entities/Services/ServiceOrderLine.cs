using BusinessCentral.Domain.Entities.Auth;
using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Services;

[Table("ServiceOrderLine", Schema = "svc")]
public sealed class ServiceOrderLine
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public long OrderId { get; set; }

    [Required]
    public int ServiceId { get; set; }

    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public int? EmployeeUserId { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(OrderId))]
    public ServiceOrder? Order { get; set; }

    [ForeignKey(nameof(ServiceId))]
    public ServiceCatalog? Service { get; set; }

    [ForeignKey(nameof(EmployeeUserId))]
    public UsersInfo? Employee { get; set; }
}

