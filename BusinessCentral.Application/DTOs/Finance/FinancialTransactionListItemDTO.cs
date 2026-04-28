namespace BusinessCentral.Application.DTOs.Finance;

public sealed class FinancialTransactionListItemDTO
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public DateTime TxDate { get; set; }
    public string Direction { get; set; } = "IN";
    public string Kind { get; set; } = "OPERATING";
    public string? CategoryCode { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public string? ThirdPartyDocument { get; set; }
    public string? ThirdPartyName { get; set; }
    public string? SourceModule { get; set; }
    public string? ReferenceType { get; set; }
    public string? ReferenceId { get; set; }
    public string? TaxCode { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
