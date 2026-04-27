using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class CompanyFinancialProfileRepository : SqlConfigServer, ICompanyFinancialProfileRepository
{
    public CompanyFinancialProfileRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<CompanyFinancialBootstrapDTO?> GetAsync(int companyId)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int)
        };

        return await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Business.sp_get_company_financial_bootstrap,
            parameters,
            reader => new CompanyFinancialBootstrapDTO
            {
                CompanyId = Convert.ToInt32(reader["CompanyId"]),
                StartupMode = reader["FinancialStartupMode"] == DBNull.Value ? null : reader["FinancialStartupMode"]?.ToString(),
                OperatingStartDateUtc = reader["FinancialOperatingStartDateUtc"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(reader["FinancialOperatingStartDateUtc"]),
                BootstrapStatus = reader["FinancialBootstrapStatus"]?.ToString() ?? "NOT_STARTED",
                Notes = reader["FinancialBootstrapNotes"] == DBNull.Value ? null : reader["FinancialBootstrapNotes"]?.ToString()
            });
    }

    public async Task<bool> UpdateAsync(int companyId, string? startupMode, DateTime? operatingStartUtc, string bootstrapStatus, string? notes)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@startup_mode", (object?)startupMode ?? DBNull.Value, SqlDbType.NVarChar, 30),
            CreateParameter("@operating_start_utc", (object?)operatingStartUtc ?? DBNull.Value, SqlDbType.DateTime2),
            CreateParameter("@bootstrap_status", bootstrapStatus, SqlDbType.NVarChar, 20),
            CreateParameter("@notes", (object?)notes ?? DBNull.Value, SqlDbType.NVarChar, 500),
        };

        var rows = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Business.sp_update_company_financial_bootstrap,
            parameters,
            reader => Convert.ToInt32(reader["RowsAffected"]));

        return rows > 0;
    }
}
