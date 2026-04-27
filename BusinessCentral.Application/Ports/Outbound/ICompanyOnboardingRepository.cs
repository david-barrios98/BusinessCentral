using BusinessCentral.Application.DTOs.Config;

namespace BusinessCentral.Application.Ports.Outbound;

public interface ICompanyOnboardingRepository
{
    Task<List<BusinessNatureDTO>> ListBusinessNaturesAsync(bool onlyActive = true);
    Task<List<BusinessNatureModuleDTO>> ListBusinessNatureModulesAsync(string natureCode);
    Task<OnboardCompanyResultDTO> OnboardCompanyAsync(OnboardCompanyRequestDTO request, string passwordHash);
}

