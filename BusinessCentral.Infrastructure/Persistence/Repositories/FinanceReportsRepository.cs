using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class FinanceReportsRepository : SqlConfigServer, IFinanceReportsRepository
{
    public FinanceReportsRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<long> CreateFinancialTransactionAsync(int companyId, CreateFinancialTransactionDTO dto)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@tx_date", dto.TxDate, SqlDbType.DateTime2),
            CreateParameter("@direction", dto.Direction, SqlDbType.NVarChar, 20),
            CreateParameter("@kind", dto.Kind, SqlDbType.NVarChar, 30),
            CreateParameter("@category_code", (object?)dto.CategoryCode ?? DBNull.Value, SqlDbType.NVarChar, 50),
            CreateParameter("@description", (object?)dto.Description ?? DBNull.Value, SqlDbType.NVarChar, 500),
            CreateParameter("@amount", dto.Amount, SqlDbType.Decimal),
            CreateParameter("@third_party_document", (object?)dto.ThirdPartyDocument ?? DBNull.Value, SqlDbType.NVarChar, 50),
            CreateParameter("@third_party_name", (object?)dto.ThirdPartyName ?? DBNull.Value, SqlDbType.NVarChar, 200),
            CreateParameter("@source_module", (object?)dto.SourceModule ?? DBNull.Value, SqlDbType.NVarChar, 50),
            CreateParameter("@reference_type", (object?)dto.ReferenceType ?? DBNull.Value, SqlDbType.NVarChar, 50),
            CreateParameter("@reference_id", (object?)dto.ReferenceId ?? DBNull.Value, SqlDbType.NVarChar, 100),
            CreateParameter("@tax_code", (object?)dto.TaxCode ?? DBNull.Value, SqlDbType.NVarChar, 50)
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Finance.sp_create_financial_transaction,
            parameters,
            reader => Convert.ToInt64(reader["InsertedId"])
        );

        return insertedId;
    }

    public async Task<List<FinancialSummaryRowDTO>> GetFinancialSummaryAsync(int companyId, DateTime fromDateUtc, DateTime toDateUtc)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@from_date", fromDateUtc, SqlDbType.DateTime2),
            CreateParameter("@to_date", toDateUtc, SqlDbType.DateTime2)
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Finance.sp_report_financial_summary,
            parameters,
            reader => new FinancialSummaryRowDTO
            {
                Day = Convert.ToDateTime(reader["Day"]),
                InAmount = Convert.ToDecimal(reader["InAmount"]),
                OutAmount = Convert.ToDecimal(reader["OutAmount"]),
                Net = Convert.ToDecimal(reader["Net"])
            });
    }

    public async Task<List<PnLRowDTO>> GetPnLAsync(int companyId, DateTime fromDateUtc, DateTime toDateUtc)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@from_date", fromDateUtc, SqlDbType.DateTime2),
            CreateParameter("@to_date", toDateUtc, SqlDbType.DateTime2)
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Finance.sp_report_pnl,
            parameters,
            reader => new PnLRowDTO
            {
                CategoryCode = reader["CategoryCode"]?.ToString() ?? string.Empty,
                Income = Convert.ToDecimal(reader["Income"]),
                Expense = Convert.ToDecimal(reader["Expense"]),
                Profit = Convert.ToDecimal(reader["Profit"])
            });
    }

    public async Task<List<TaxSummaryRowDTO>> GetTaxSummaryCoAsync(int companyId, DateTime fromDateUtc, DateTime toDateUtc)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@from_date", fromDateUtc, SqlDbType.DateTime2),
            CreateParameter("@to_date", toDateUtc, SqlDbType.DateTime2)
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Finance.sp_report_tax_summary_co,
            parameters,
            reader => new TaxSummaryRowDTO
            {
                TaxCode = reader["TaxCode"]?.ToString() ?? string.Empty,
                TaxIn = Convert.ToDecimal(reader["TaxIn"]),
                TaxOut = Convert.ToDecimal(reader["TaxOut"]),
                NetPayable = Convert.ToDecimal(reader["NetPayable"])
            });
    }

    public async Task<RentaAnnualSummaryDTO> GetRentaAnnualCoAsync(int companyId, int year)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@year", year, SqlDbType.Int)
        };

        var result = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Finance.sp_report_renta_annual_co,
            parameters,
            reader => new RentaAnnualSummaryDTO
            {
                Year = Convert.ToInt32(reader["Year"]),
                TotalIncome = Convert.ToDecimal(reader["TotalIncome"]),
                TotalExpense = Convert.ToDecimal(reader["TotalExpense"]),
                TaxesPaid = Convert.ToDecimal(reader["TaxesPaid"]),
                TaxesCollected = Convert.ToDecimal(reader["TaxesCollected"]),
                NetIncome = Convert.ToDecimal(reader["NetIncome"])
            });

        return result ?? new RentaAnnualSummaryDTO { Year = year };
    }
}

