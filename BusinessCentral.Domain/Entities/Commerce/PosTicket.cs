using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Commerce;

[Table("PosTicket", Schema = "com")]
public sealed class PosTicket
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    public long? CashSessionId { get; set; }
    public DateTime TicketDate { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "open";

    public decimal Total { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(CashSessionId))]
    public CashSession? CashSession { get; set; }
}

