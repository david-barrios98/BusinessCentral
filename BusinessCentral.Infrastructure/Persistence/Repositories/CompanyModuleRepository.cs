using BusinessCentral.Application.DTOs.Config;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class CompanyModuleRepository : SqlConfigServer, ICompanyModuleRepository
{
    public CompanyModuleRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<List<ModuleDTO>> ListModulesAsync(bool onlyActive = true)
    {
        var parameters = new[]
        {
            CreateParameter("@only_active", onlyActive, SqlDbType.Bit)
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Config.sp_list_modules,
            parameters,
            reader => new ModuleDTO
            {
                Id = Convert.ToInt32(reader["Id"]),
                Code = reader["Code"]?.ToString(),
                Name = reader["Name"]?.ToString() ?? string.Empty,
                Description = reader["Description"]?.ToString(),
                Active = Convert.ToBoolean(reader["Active"])
            });
    }

    public async Task<List<CompanyModuleDTO>> ListCompanyModulesAsync(int companyId)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int)
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Config.sp_list_company_modules,
            parameters,
            reader => new CompanyModuleDTO
            {
                CompanyId = Convert.ToInt32(reader["CompanyId"]),
                ModuleId = Convert.ToInt32(reader["ModuleId"]),
                ModuleCode = reader["ModuleCode"]?.ToString(),
                ModuleName = reader["ModuleName"]?.ToString() ?? string.Empty,
                IsEnabled = Convert.ToBoolean(reader["IsEnabled"])
            });
    }

    public async Task<bool> SetCompanyModuleAsync(int companyId, string moduleCode, bool isEnabled)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@module_code", moduleCode, SqlDbType.NVarChar, 50),
            CreateParameter("@is_enabled", isEnabled, SqlDbType.Bit)
        };

        var success = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Config.sp_set_company_module,
            parameters,
            reader => Convert.ToBoolean(reader["Success"])
        );

        return success == true;
    }

    public async Task<bool> IsCompanyModuleEnabledAsync(int companyId, string moduleCode)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@module_code", moduleCode, SqlDbType.NVarChar, 50)
        };

        var enabled = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Config.sp_is_company_module_enabled,
            parameters,
            reader => Convert.ToBoolean(reader["IsEnabled"])
        );

        return enabled == true;
    }
}

