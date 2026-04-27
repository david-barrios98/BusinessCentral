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
    public int? ClosedByUserId { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "open";

    public decimal OpeningAmount { get; set; }
    /// <summary>Valor contado al cierre (arqueo).</summary>
    public decimal? CountedClosingAmount { get; set; }
    /// <summary>Valor esperado según apertura + movimientos + pagos en efectivo.</summary>
    public decimal? ExpectedClosingAmount { get; set; }
    /// <summary>Diferencia = contado - esperado.</summary>
    public decimal? DifferenceAmount { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(OpenedByUserId))]
    public UsersInfo? OpenedByUser { get; set; }

    [ForeignKey(nameof(ClosedByUserId))]
    public UsersInfo? ClosedByUser { get; set; }
}

