namespace BusinessCentral.Application.DTOs.Finance;

public sealed class FinancialSummaryRowDTO
{
    public DateTime Day { get; set; }
    public decimal InAmount { get; set; }
    public decimal OutAmount { get; set; }
    public decimal Net { get; set; }
}

