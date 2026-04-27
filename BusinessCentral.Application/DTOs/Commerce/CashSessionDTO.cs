namespace BusinessCentral.Application.DTOs.Commerce;

public sealed class CashSessionDTO
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public DateTime OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public int? OpenedByUserId { get; set; }
    public string Status { get; set; } = "open";
    public decimal OpeningAmount { get; set; }
    public decimal? ClosingAmount { get; set; }
}

