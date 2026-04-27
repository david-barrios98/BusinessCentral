using BusinessCentral.Domain.Entities.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Hr;

[Table("EmployeeAvailabilityException", Schema = "hr")]
public sealed class EmployeeAvailabilityException
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public int UserId { get; set; }

    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }

    public bool IsAvailable { get; set; } = false; // false = no disponible (vacaciones), true = disponible extra

    [MaxLength(300)]
    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public UsersInfo? User { get; set; }
}

