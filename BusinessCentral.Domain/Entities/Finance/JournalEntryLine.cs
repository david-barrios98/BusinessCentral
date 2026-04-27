using BusinessCentral.Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCentral.Domain.Entities.Finance;

[Table("JournalEntryLine", Schema = "fin")]
public sealed class JournalEntryLine
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required]
    public long JournalEntryId { get; set; }

    [Required]
    public long AccountId { get; set; }

    public decimal Debit { get; set; }
    public decimal Credit { get; set; }

    [MaxLength(50)]
    public string? ThirdPartyDocument { get; set; }

    [MaxLength(200)]
    public string? ThirdPartyName { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Companies? Company { get; set; }

    [ForeignKey(nameof(JournalEntryId))]
    public JournalEntry? JournalEntry { get; set; }

    [ForeignKey(nameof(AccountId))]
    public Account? Account { get; set; }
}

