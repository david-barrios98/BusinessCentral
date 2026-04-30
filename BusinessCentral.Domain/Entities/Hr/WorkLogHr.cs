using BusinessCentral.Domain.Entities.Auth;
using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Hr;

[Table("WorkLogHr", Schema = "hr")]
public sealed class WorkLogHr
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public DateTime WorkDate { get; set; }

    [Required]
    public int PaySchemeId { get; set; }

    public decimal Quantity { get; set; } = 1;
    public string? Unit { get; set; }
    public string? ReferenceType { get; set; }
    public string? ReferenceId { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(UserId))]
    public UsersInfo? User { get; set; }

    [ForeignKey(nameof(PaySchemeId))]
    public PayScheme? PayScheme { get; set; }
}

