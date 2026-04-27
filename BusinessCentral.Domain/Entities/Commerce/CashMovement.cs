using BusinessCentral.Domain.Entities.Auth;
using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Commerce;

[Table("CashMovement", Schema = "com")]
public sealed class CashMovement
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public long CashSessionId { get; set; }

    /// <summary>IN / OUT</summary>
    [Required]
    [MaxLength(10)]
    public string Direction { get; set; } = "OUT";

    [MaxLength(30)]
    public string? ReasonCode { get; set; } // WITHDRAWAL / EXPENSE / CHANGE / DEPOSIT ...

    public decimal Amount { get; set; }

    [MaxLength(100)]
    public string? ReferenceType { get; set; }

    [MaxLength(100)]
    public string? ReferenceId { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public int? PerformedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(CashSessionId))]
    public CashSession? CashSession { get; set; }

    [ForeignKey(nameof(PerformedByUserId))]
    public UsersInfo? PerformedByUser { get; set; }
}

