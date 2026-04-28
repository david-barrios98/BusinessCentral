using BusinessCentral.Application.DTOs.Config;

namespace BusinessCentral.Application.Ports.Outbound;

public interface ICompanyOnboardingRepository
{
    Task<List<BusinessNatureDTO>> ListBusinessNaturesAsync(bool onlyActive = true);
    Task<List<BusinessNatureModuleDTO>> ListBusinessNatureModulesAsync(string natureCode);
    Task<List<CompanyBusinessNatureDTO>> ListCompanyBusinessNaturesAsync(int companyId);
    Task<bool> SetCompanyBusinessNatureAsync(int companyId, string natureCode, bool isPrimary, bool enabled);
    Task<OnboardCompanyResultDTO> OnboardCompanyAsync(OnboardCompanyRequestDTO request, string passwordHash);
    Task<List<FacilityTypeDTO>> ListFacilityTypesAsync(bool onlyActive = true);
}

