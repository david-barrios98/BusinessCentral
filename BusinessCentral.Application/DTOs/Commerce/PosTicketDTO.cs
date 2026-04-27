namespace BusinessCentral.Application.DTOs.Commerce;

public sealed class PosTicketDTO
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public long? CashSessionId { get; set; }
    public DateTime TicketDate { get; set; }
    public string Status { get; set; } = "open";
    public decimal Total { get; set; }
}

