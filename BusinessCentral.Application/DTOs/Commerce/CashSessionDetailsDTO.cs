namespace BusinessCentral.Application.DTOs.Commerce;

public sealed class CashSessionDetailsDTO
{
    public CashSessionHeaderDTO Session { get; set; } = new();
    public List<CashMovementDTO> Movements { get; set; } = new();
}

public sealed class CashSessionHeaderDTO
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public DateTime OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public int? OpenedByUserId { get; set; }
    public int? ClosedByUserId { get; set; }
    public string Status { get; set; } = "open";
    public decimal OpeningAmount { get; set; }
    public decimal? ExpectedClosingAmount { get; set; }
    public decimal? CountedClosingAmount { get; set; }
    public decimal? DifferenceAmount { get; set; }
}

public sealed class CashMovementDTO
{
    public long Id { get; set; }
    public long CashSessionId { get; set; }
    public string Direction { get; set; } = "OUT";
    public string? ReasonCode { get; set; }
    public decimal Amount { get; set; }
    public string? ReferenceType { get; set; }
    public string? ReferenceId { get; set; }
    public string? Notes { get; set; }
    public int? PerformedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class CashSessionCloseResultDTO
{
    public bool Success { get; set; }
    public decimal Expected { get; set; }
    public decimal Difference { get; set; }
}

