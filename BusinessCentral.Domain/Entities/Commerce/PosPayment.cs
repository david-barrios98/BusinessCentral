using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Commerce;

[Table("PosPayment", Schema = "com")]
public sealed class PosPayment
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public long TicketId { get; set; }

    [Required]
    [MaxLength(30)]
    public string Method { get; set; } = "CASH";

    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(TicketId))]
    public PosTicket? Ticket { get; set; }
}

