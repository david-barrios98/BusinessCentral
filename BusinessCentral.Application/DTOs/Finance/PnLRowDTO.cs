namespace BusinessCentral.Application.DTOs.Finance;

public sealed class PnLRowDTO
{
    public string CategoryCode { get; set; } = string.Empty;
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
    public decimal Profit { get; set; }
}

