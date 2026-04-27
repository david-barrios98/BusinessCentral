using BusinessCentral.Application.DTOs.Config;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class CompanyOnboardingRepository : SqlConfigServer, ICompanyOnboardingRepository
{
    public CompanyOnboardingRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<List<BusinessNatureDTO>> ListBusinessNaturesAsync(bool onlyActive = true)
    {
        var parameters = new[]
        {
            CreateParameter("@only_active", onlyActive, SqlDbType.Bit)
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Config.sp_list_business_natures,
            parameters,
            reader => new BusinessNatureDTO
            {
                Id = Convert.ToInt32(reader["Id"]),
                Code = reader["Code"]?.ToString() ?? string.Empty,
                Name = reader["Name"]?.ToString() ?? string.Empty,
                Description = reader["Description"]?.ToString(),
                Active = Convert.ToBoolean(reader["Active"])
            });
    }

    public async Task<List<BusinessNatureModuleDTO>> ListBusinessNatureModulesAsync(string natureCode)
    {
        var parameters = new[]
        {
            CreateParameter("@nature_code", natureCode, SqlDbType.NVarChar, 50)
        };

        // SP puede devolver un SELECT de error (Success/Message). Nosotros leemos el dataset "bueno".
        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Config.sp_list_business_nature_modules,
            parameters,
            reader => new BusinessNatureModuleDTO
            {
                BusinessNatureId = Convert.ToInt32(reader["BusinessNatureId"]),
                ModuleId = Convert.ToInt32(reader["ModuleId"]),
                ModuleCode = reader["ModuleCode"]?.ToString(),
                ModuleName = reader["ModuleName"]?.ToString() ?? string.Empty,
                IsDefaultEnabled = Convert.ToBoolean(reader["IsDefaultEnabled"]),
                SortOrder = Convert.ToInt32(reader["SortOrder"])
            });
    }

    public async Task<List<CompanyBusinessNatureDTO>> ListCompanyBusinessNaturesAsync(int companyId)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int)
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Config.sp_list_company_business_natures,
            parameters,
            reader => new CompanyBusinessNatureDTO
            {
                CompanyId = Convert.ToInt32(reader["CompanyId"]),
                BusinessNatureId = Convert.ToInt32(reader["BusinessNatureId"]),
                NatureCode = reader["NatureCode"]?.ToString() ?? string.Empty,
                NatureName = reader["NatureName"]?.ToString() ?? string.Empty,
                IsPrimary = Convert.ToBoolean(reader["IsPrimary"]),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
            });
    }

    public async Task<bool> SetCompanyBusinessNatureAsync(int companyId, string natureCode, bool isPrimary, bool enabled)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@nature_code", natureCode, SqlDbType.NVarChar, 50),
            CreateParameter("@is_primary", isPrimary, SqlDbType.Bit),
            CreateParameter("@enabled", enabled, SqlDbType.Bit)
        };

        var ok = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Config.sp_set_company_business_nature,
            parameters,
            reader => Convert.ToBoolean(reader["Success"])
        );

        return ok == true;
    }

    public async Task<OnboardCompanyResultDTO> OnboardCompanyAsync(OnboardCompanyRequestDTO request, string passwordHash)
    {
        var parameters = new[]
        {
            CreateParameter("@CompanyName", request.CompanyName, SqlDbType.NVarChar, 200),
            CreateParameter("@TradeName", (object?)request.TradeName ?? DBNull.Value, SqlDbType.NVarChar, 200),
            CreateParameter("@Subdomain", (object?)request.Subdomain ?? DBNull.Value, SqlDbType.NVarChar, 50),
            CreateParameter("@DocumentTypeId", (object?)request.DocumentTypeId ?? DBNull.Value, SqlDbType.Int),
            CreateParameter("@DocumentNumber", (object?)request.DocumentNumber ?? DBNull.Value, SqlDbType.NVarChar, 50),
            CreateParameter("@Email", (object?)request.Email ?? DBNull.Value, SqlDbType.NVarChar, 150),
            CreateParameter("@Phone", (object?)request.Phone ?? DBNull.Value, SqlDbType.NVarChar, 20),
            CreateParameter("@BusinessNatureCode", request.BusinessNatureCode, SqlDbType.NVarChar, 50),

            CreateParameter("@MembershipPlanId", request.MembershipPlanId, SqlDbType.Int),
            CreateParameter("@StartDate", (object?)request.StartDateUtc ?? DBNull.Value, SqlDbType.DateTime2),
            CreateParameter("@AutoRenew", request.AutoRenew, SqlDbType.Bit),

            CreateParameter("@FacilityTypeId", request.FacilityTypeId, SqlDbType.Int),
            CreateParameter("@FacilityName", request.FacilityName, SqlDbType.NVarChar, 200),
            CreateParameter("@FacilityCode", (object?)request.FacilityCode ?? DBNull.Value, SqlDbType.NVarChar, 200),
            CreateParameter("@FacilityEmail", (object?)request.FacilityEmail ?? DBNull.Value, SqlDbType.NVarChar, 150),
            CreateParameter("@FacilityPhone", (object?)request.FacilityPhone ?? DBNull.Value, SqlDbType.NVarChar, 20),

            CreateParameter("@OwnerDocumentTypeId", request.OwnerDocumentTypeId, SqlDbType.Int),
            CreateParameter("@OwnerDocumentNumber", request.OwnerDocumentNumber, SqlDbType.NVarChar, 50),
            CreateParameter("@OwnerFirstName", request.OwnerFirstName, SqlDbType.NVarChar, 150),
            CreateParameter("@OwnerLastName", request.OwnerLastName, SqlDbType.NVarChar, 150),
            CreateParameter("@OwnerEmail", request.OwnerEmail, SqlDbType.NVarChar, 150),
            CreateParameter("@OwnerPhone", request.OwnerPhone, SqlDbType.NVarChar, 20),
            CreateParameter("@OwnerPasswordHash", passwordHash, SqlDbType.NVarChar, 255),
            CreateParameter("@OwnerRoleId", request.OwnerRoleId, SqlDbType.Int)
        };

        var result = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Config.sp_onboard_company,
            parameters,
            reader => new OnboardCompanyResultDTO
            {
                Success = Convert.ToBoolean(reader["Success"]),
                CompanyId = Convert.ToInt32(reader["CompanyId"]),
                OwnerUserId = Convert.ToInt32(reader["OwnerUserId"]),
                BusinessNatureId = Convert.ToInt32(reader["BusinessNatureId"]),
                MembershipPlanId = Convert.ToInt32(reader["MembershipPlanId"]),
                StartDate = Convert.ToDateTime(reader["StartDate"]),
                EndDate = Convert.ToDateTime(reader["EndDate"])
            });

        return result ?? new OnboardCompanyResultDTO { Success = false };
    }
}

