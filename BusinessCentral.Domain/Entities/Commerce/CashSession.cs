using BusinessCentral.Domain.Entities.Auth;
using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Commerce;

[Table("CashSession", Schema = "com")]
public sealed class CashSession
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    public DateTime OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    public int? OpenedByUserId { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "open";

    public decimal OpeningAmount { get; set; }
    public decimal? ClosingAmount { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(OpenedByUserId))]
    public UsersInfo? OpenedByUser { get; set; }
}

