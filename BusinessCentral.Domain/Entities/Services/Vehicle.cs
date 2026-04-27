using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Services;

[Table("Vehicle", Schema = "svc")]
public sealed class Vehicle
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    public long? CustomerId { get; set; }

    [MaxLength(50)]
    public string? Type { get; set; }

    [MaxLength(20)]
    public string? Plate { get; set; }

    [MaxLength(50)]
    public string? Brand { get; set; }

    [MaxLength(50)]
    public string? Model { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public Customer? Customer { get; set; }
}

