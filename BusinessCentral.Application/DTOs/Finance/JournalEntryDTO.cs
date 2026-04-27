namespace BusinessCentral.Application.DTOs.Finance;

public sealed class JournalEntryDTO
{
    public long Id { get; set; }
    public int CompanyId { get; set; }
    public DateTime EntryDate { get; set; }
    public string? EntryType { get; set; }
    public string? ReferenceType { get; set; }
    public string? ReferenceId { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "draft";
    public int? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<JournalEntryLineDTO> Lines { get; set; } = new();
}

