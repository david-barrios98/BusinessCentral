using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Services;

[Table("ServiceOrder", Schema = "svc")]
public sealed class ServiceOrder
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    public long? CustomerId { get; set; }
    public long? VehicleId { get; set; }

    public DateTime OrderDate { get; set; }
    public string? VehicleType { get; set; }
    public string? Plate { get; set; }
    public string? CustomerName { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "open";

    [MaxLength(30)]
    public string? FulfillmentMethodCode { get; set; } // IN_STORE / ON_SITE / OTHER_LOCATION

    [MaxLength(500)]
    public string? FulfillmentDetails { get; set; }

    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public Customer? Customer { get; set; }

    [ForeignKey(nameof(VehicleId))]
    public Vehicle? Vehicle { get; set; }
}

