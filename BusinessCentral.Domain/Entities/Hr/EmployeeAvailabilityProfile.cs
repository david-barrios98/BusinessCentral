using BusinessCentral.Domain.Entities.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Hr;

[Table("EmployeeAvailabilityProfile", Schema = "hr")]
public sealed class EmployeeAvailabilityProfile
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public int UserId { get; set; }

    [MaxLength(80)]
    public string? TimeZone { get; set; } // opcional (ej: America/Bogota)

    public int? MaxServicesPerDay { get; set; }

    public bool Active { get; set; } = true;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public UsersInfo? User { get; set; }
}

