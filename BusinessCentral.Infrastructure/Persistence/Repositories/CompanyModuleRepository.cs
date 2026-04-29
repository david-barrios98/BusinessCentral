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

    public async Task<ModuleDTO?> GetModuleByIdAsync(int id)
    {
        var parameters = new[]
        {
            CreateParameter("@Id", id, SqlDbType.Int)
        };

        return await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Config.sp_get_module_by_id,
            parameters,
            r => new ModuleDTO
            {
                Id = Convert.ToInt32(r["Id"]),
                Code = r["Code"]?.ToString(),
                Name = r["Name"]?.ToString() ?? string.Empty,
                Description = r["Description"]?.ToString(),
                Active = Convert.ToBoolean(r["Active"])
            });
    }

    public async Task<int> UpsertModuleAsync(UpsertModuleRequestDTO request)
    {
        object idParam = request.Id is > 0 ? request.Id.Value : DBNull.Value;

        var parameters = new[]
        {
            CreateParameter("@Id", idParam, SqlDbType.Int),
            CreateParameter("@Code", request.Code.Trim(), SqlDbType.NVarChar, 50),
            CreateParameter("@Name", request.Name.Trim(), SqlDbType.NVarChar, 100),
            CreateParameter("@Description", (object?)request.Description ?? DBNull.Value, SqlDbType.NVarChar, 250),
            CreateParameter("@Active", request.Active, SqlDbType.Bit)
        };

        int? newId = await ExecuteStoredProcedureSingleAsync<int>(
            StoredProcedures.Config.sp_upsert_module,
            parameters,
            r => Convert.ToInt32(r["Id"]));

        return newId ?? 0;
    }

    public async Task DeleteModuleAsync(int id)
    {
        var parameters = new[]
        {
            CreateParameter("@Id", id, SqlDbType.Int)
        };

        await ExecuteStoredProcedureNonQueryAsync(
            StoredProcedures.Config.sp_delete_module,
            parameters);
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

