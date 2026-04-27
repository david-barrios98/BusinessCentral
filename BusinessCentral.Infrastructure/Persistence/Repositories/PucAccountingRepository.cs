using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class PucAccountingRepository : SqlConfigServer, IPucAccountingRepository
{
    public PucAccountingRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<List<AccountDTO>> ListAccountsAsync(int companyId, bool onlyActive = true, string? q = null)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@only_active", onlyActive, SqlDbType.Bit),
            CreateParameter("@q", (object?)q ?? DBNull.Value, SqlDbType.NVarChar, 100)
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Finance.sp_list_accounts,
            parameters,
            reader => new AccountDTO
            {
                Id = Convert.ToInt64(reader["Id"]),
                CompanyId = Convert.ToInt32(reader["CompanyId"]),
                Code = reader["Code"]?.ToString() ?? string.Empty,
                Name = reader["Name"]?.ToString() ?? string.Empty,
                Nature = reader["Nature"]?.ToString() ?? "D",
                Level = Convert.ToInt32(reader["Level"]),
                ParentAccountId = reader["ParentAccountId"] == DBNull.Value ? null : Convert.ToInt64(reader["ParentAccountId"]),
                IsAuxiliary = Convert.ToBoolean(reader["IsAuxiliary"]),
                Active = Convert.ToBoolean(reader["Active"])
            });
    }

    public async Task<long?> GetAccountIdByCodeAsync(int companyId, string accountCode)
    {
        if (string.IsNullOrWhiteSpace(accountCode))
            return null;

        var code = accountCode.Trim();
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@account_code", code, SqlDbType.NVarChar, 20)
        };

        var id = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Finance.sp_get_account_id_by_code,
            parameters,
            reader => Convert.ToInt64(reader["Id"]));

        return id;
    }

    public async Task<long> CreateJournalEntryAsync(int companyId, DateTime entryDate, string? entryType, string? referenceType, string? referenceId, string? description, int? createdByUserId)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@entry_date", entryDate, SqlDbType.DateTime2),
            CreateParameter("@entry_type", (object?)entryType ?? DBNull.Value, SqlDbType.NVarChar, 30),
            CreateParameter("@reference_type", (object?)referenceType ?? DBNull.Value, SqlDbType.NVarChar, 50),
            CreateParameter("@reference_id", (object?)referenceId ?? DBNull.Value, SqlDbType.NVarChar, 100),
            CreateParameter("@description", (object?)description ?? DBNull.Value, SqlDbType.NVarChar, 500),
            CreateParameter("@created_by_user_id", (object?)createdByUserId ?? DBNull.Value, SqlDbType.Int)
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Finance.sp_create_journal_entry,
            parameters,
            reader => Convert.ToInt64(reader["InsertedId"])
        );

        return insertedId;
    }

    public async Task<long> AddJournalEntryLineAsync(int companyId, long journalEntryId, long accountId, decimal debit, decimal credit, string? thirdPartyDocument, string? thirdPartyName, string? notes)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@journal_entry_id", journalEntryId, SqlDbType.BigInt),
            CreateParameter("@account_id", accountId, SqlDbType.BigInt),
            CreateParameter("@debit", debit, SqlDbType.Decimal),
            CreateParameter("@credit", credit, SqlDbType.Decimal),
            CreateParameter("@third_party_document", (object?)thirdPartyDocument ?? DBNull.Value, SqlDbType.NVarChar, 50),
            CreateParameter("@third_party_name", (object?)thirdPartyName ?? DBNull.Value, SqlDbType.NVarChar, 200),
            CreateParameter("@notes", (object?)notes ?? DBNull.Value, SqlDbType.NVarChar, 500)
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Finance.sp_add_journal_entry_line,
            parameters,
            reader => Convert.ToInt64(reader["InsertedId"])
        );

        return insertedId;
    }

    public async Task<JournalEntryDTO?> GetJournalEntryAsync(int companyId, long journalEntryId)
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(StoredProcedures.Finance.sp_get_journal_entry, connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add(CreateParameter("@company_id", companyId, SqlDbType.Int));
        command.Parameters.Add(CreateParameter("@journal_entry_id", journalEntryId, SqlDbType.BigInt));

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        var header = new JournalEntryDTO
        {
            Id = Convert.ToInt64(reader["Id"]),
            CompanyId = Convert.ToInt32(reader["CompanyId"]),
            EntryDate = Convert.ToDateTime(reader["EntryDate"]),
            EntryType = reader["EntryType"] == DBNull.Value ? null : reader["EntryType"]?.ToString(),
            ReferenceType = reader["ReferenceType"] == DBNull.Value ? null : reader["ReferenceType"]?.ToString(),
            ReferenceId = reader["ReferenceId"] == DBNull.Value ? null : reader["ReferenceId"]?.ToString(),
            Description = reader["Description"] == DBNull.Value ? null : reader["Description"]?.ToString(),
            Status = reader["Status"]?.ToString() ?? "draft",
            CreatedByUserId = reader["CreatedByUserId"] == DBNull.Value ? null : Convert.ToInt32(reader["CreatedByUserId"]),
            CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
            UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
        };

        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                header.Lines.Add(new JournalEntryLineDTO
                {
                    Id = Convert.ToInt64(reader["Id"]),
                    JournalEntryId = Convert.ToInt64(reader["JournalEntryId"]),
                    AccountId = Convert.ToInt64(reader["AccountId"]),
                    AccountCode = reader["AccountCode"]?.ToString() ?? string.Empty,
                    AccountName = reader["AccountName"]?.ToString() ?? string.Empty,
                    Debit = Convert.ToDecimal(reader["Debit"]),
                    Credit = Convert.ToDecimal(reader["Credit"]),
                    ThirdPartyDocument = reader["ThirdPartyDocument"] == DBNull.Value ? null : reader["ThirdPartyDocument"]?.ToString(),
                    ThirdPartyName = reader["ThirdPartyName"] == DBNull.Value ? null : reader["ThirdPartyName"]?.ToString(),
                    Notes = reader["Notes"] == DBNull.Value ? null : reader["Notes"]?.ToString(),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }
        }

        return header;
    }

    public async Task<bool> PostJournalEntryAsync(int companyId, long journalEntryId)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@journal_entry_id", journalEntryId, SqlDbType.BigInt)
        };

        var ok = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Finance.sp_post_journal_entry,
            parameters,
            reader => Convert.ToBoolean(reader["Success"])
        );

        return ok == true;
    }

    public async Task<List<TrialBalanceRowDTO>> GetTrialBalanceAsync(int companyId, DateTime fromDateUtc, DateTime toDateUtc)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@from_date", fromDateUtc, SqlDbType.DateTime2),
            CreateParameter("@to_date", toDateUtc, SqlDbType.DateTime2)
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Finance.sp_report_trial_balance,
            parameters,
            reader => new TrialBalanceRowDTO
            {
                AccountCode = reader["AccountCode"]?.ToString() ?? string.Empty,
                AccountName = reader["AccountName"]?.ToString() ?? string.Empty,
                Debit = Convert.ToDecimal(reader["Debit"]),
                Credit = Convert.ToDecimal(reader["Credit"]),
                Balance = Convert.ToDecimal(reader["Balance"])
            });
    }

    public async Task<List<PucClassSummaryRowDTO>> GetIncomeStatementAsync(int companyId, DateTime fromDateUtc, DateTime toDateUtc)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@from_date", fromDateUtc, SqlDbType.DateTime2),
            CreateParameter("@to_date", toDateUtc, SqlDbType.DateTime2)
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Finance.sp_report_income_statement_puc,
            parameters,
            reader => new PucClassSummaryRowDTO
            {
                ClassCode = reader["ClassCode"]?.ToString() ?? string.Empty,
                ClassName = reader["ClassName"]?.ToString() ?? string.Empty,
                Debit = Convert.ToDecimal(reader["Debit"]),
                Credit = Convert.ToDecimal(reader["Credit"]),
                Balance = Convert.ToDecimal(reader["Net"])
            });
    }

    public async Task<List<PucClassSummaryRowDTO>> GetBalanceSheetAsync(int companyId, DateTime toDateUtc)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@to_date", toDateUtc, SqlDbType.DateTime2)
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Finance.sp_report_balance_sheet_puc,
            parameters,
            reader => new PucClassSummaryRowDTO
            {
                ClassCode = reader["ClassCode"]?.ToString() ?? string.Empty,
                ClassName = reader["ClassName"]?.ToString() ?? string.Empty,
                Debit = Convert.ToDecimal(reader["Debit"]),
                Credit = Convert.ToDecimal(reader["Credit"]),
                Balance = Convert.ToDecimal(reader["Balance"])
            });
    }
}

