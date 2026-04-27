using BusinessCentral.Application.DTOs.Finance;

namespace BusinessCentral.Application.Ports.Outbound;

public interface ICompanyFinancialProfileRepository
{
    Task<CompanyFinancialBootstrapDTO?> GetAsync(int companyId);
    Task<bool> UpdateAsync(int companyId, string? startupMode, DateTime? operatingStartUtc, string bootstrapStatus, string? notes);
}
