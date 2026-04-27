using BusinessCentral.Domain.Entities.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Hr;

[Table("EmployeeAvailabilitySlot", Schema = "hr")]
public sealed class EmployeeAvailabilitySlot
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public int UserId { get; set; }

    /// <summary>0=Sunday ... 6=Saturday</summary>
    public int DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public int? MaxServicesInSlot { get; set; }

    public bool Active { get; set; } = true;

    [ForeignKey(nameof(UserId))]
    public UsersInfo? User { get; set; }
}

