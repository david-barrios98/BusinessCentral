namespace BusinessCentral.Application.DTOs.Finance;

public sealed class PucClassSummaryRowDTO
{
    public string ClassCode { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }
}

