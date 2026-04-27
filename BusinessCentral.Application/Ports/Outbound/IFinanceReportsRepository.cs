using BusinessCentral.Application.DTOs.Finance;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IFinanceReportsRepository
{
    Task<long> CreateFinancialTransactionAsync(int companyId, CreateFinancialTransactionDTO dto);
    Task<List<FinancialSummaryRowDTO>> GetFinancialSummaryAsync(int companyId, DateTime fromDateUtc, DateTime toDateUtc);
    Task<List<PnLRowDTO>> GetPnLAsync(int companyId, DateTime fromDateUtc, DateTime toDateUtc);
    Task<List<TaxSummaryRowDTO>> GetTaxSummaryCoAsync(int companyId, DateTime fromDateUtc, DateTime toDateUtc);
    Task<RentaAnnualSummaryDTO> GetRentaAnnualCoAsync(int companyId, int year);
}

