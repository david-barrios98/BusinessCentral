using BusinessCentral.Application.DTOs.Config;

namespace BusinessCentral.Application.Ports.Outbound;

public interface ICompanyModuleRepository
{
    Task<List<ModuleDTO>> ListModulesAsync(bool onlyActive = true);
    Task<List<CompanyModuleDTO>> ListCompanyModulesAsync(int companyId);
    Task<bool> SetCompanyModuleAsync(int companyId, string moduleCode, bool isEnabled);
    Task<bool> IsCompanyModuleEnabledAsync(int companyId, string moduleCode);
}

