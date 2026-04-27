namespace BusinessCentral.Application.DTOs.Finance;

public sealed class TaxSummaryRowDTO
{
    public string TaxCode { get; set; } = string.Empty;
    public decimal TaxIn { get; set; }
    public decimal TaxOut { get; set; }
    public decimal NetPayable { get; set; }
}

