namespace BusinessCentral.Application.DTOs.Commerce;

public sealed class PosPaymentDTO
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public long TicketId { get; set; }
    public string Method { get; set; } = "CASH";
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }
}

