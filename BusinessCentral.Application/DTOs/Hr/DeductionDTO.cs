namespace BusinessCentral.Application.DTOs.Hr;

public sealed class DeductionDTO
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string? Type { get; set; }
    public string? Notes { get; set; }
}

