namespace BusinessCentral.Application.DTOs.Hr;

public sealed class LoanAdvanceDTO
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "open";
}

