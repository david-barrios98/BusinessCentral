namespace BusinessCentral.Application.DTOs.Finance;

public sealed class JournalEntryLineDTO
{
    public long Id { get; set; }
    public long JournalEntryId { get; set; }
    public long AccountId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public string? ThirdPartyDocument { get; set; }
    public string? ThirdPartyName { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

