namespace BusinessCentral.Application.DTOs.Finance;

public sealed class TrialBalanceRowDTO
{
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }
}

