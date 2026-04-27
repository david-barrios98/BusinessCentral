using BusinessCentral.Application.DTOs.Finance;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IPucAccountingRepository
{
    Task<List<AccountDTO>> ListAccountsAsync(int companyId, bool onlyActive = true, string? q = null);
    /// <summary>Resuelve Id de cuenta auxiliar por código PUC; null si no existe o inactiva.</summary>
    Task<long?> GetAccountIdByCodeAsync(int companyId, string accountCode);
    Task<long> CreateJournalEntryAsync(int companyId, DateTime entryDate, string? entryType, string? referenceType, string? referenceId, string? description, int? createdByUserId);
    Task<long> AddJournalEntryLineAsync(int companyId, long journalEntryId, long accountId, decimal debit, decimal credit, string? thirdPartyDocument, string? thirdPartyName, string? notes);
    Task<JournalEntryDTO?> GetJournalEntryAsync(int companyId, long journalEntryId);
    Task<bool> PostJournalEntryAsync(int companyId, long journalEntryId);

    Task<List<TrialBalanceRowDTO>> GetTrialBalanceAsync(int companyId, DateTime fromDateUtc, DateTime toDateUtc);
    Task<List<PucClassSummaryRowDTO>> GetIncomeStatementAsync(int companyId, DateTime fromDateUtc, DateTime toDateUtc);
    Task<List<PucClassSummaryRowDTO>> GetBalanceSheetAsync(int companyId, DateTime toDateUtc);
}

