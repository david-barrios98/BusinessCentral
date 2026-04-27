namespace BusinessCentral.Application.DTOs.Commerce;

public sealed class PosTicketReceiptDTO
{
    public PosTicketHeaderDTO Ticket { get; set; } = new();
    public List<PosTicketReceiptLineDTO> Lines { get; set; } = new();
    public List<PosTicketPaymentDTO> Payments { get; set; } = new();
}

public sealed class PosTicketHeaderDTO
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public long? CashSessionId { get; set; }
    public DateTime TicketDate { get; set; }
    public string Status { get; set; } = "open";
    public decimal Total { get; set; }

    public string? FulfillmentMethodCode { get; set; }
    public string? FulfillmentDetails { get; set; }
}

public sealed class PosTicketReceiptLineDTO
{
    public long Id { get; set; }
    public long TicketId { get; set; }
    public int ProductId { get; set; }
    public string ProductSku { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public sealed class PosTicketPaymentDTO
{
    public long Id { get; set; }
    public long TicketId { get; set; }
    public string Method { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }
}

