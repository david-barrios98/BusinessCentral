using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Commerce;

[Table("PosTicketLine", Schema = "com")]
public sealed class PosTicketLine
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public long TicketId { get; set; }

    [Required]
    public int ProductId { get; set; }

    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(TicketId))]
    public PosTicket? Ticket { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }
}

