using BusinessCentral.Application.DTOs.Config;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class FulfillmentMethodRepository : SqlConfigServer, IFulfillmentMethodRepository
{
    public FulfillmentMethodRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<List<FulfillmentMethodDTO>> ListMethodsAsync(bool onlyActive = true)
    {
        var parameters = new[]
        {
            CreateParameter("@only_active", onlyActive, SqlDbType.Bit),
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Config.sp_list_fulfillment_methods,
            parameters,
            r => new FulfillmentMethodDTO
            {
                Id = Convert.ToInt32(r["Id"]),
                Code = r["Code"]?.ToString() ?? string.Empty,
                Name = r["Name"]?.ToString() ?? string.Empty,
                AppliesTo = r["AppliesTo"]?.ToString() ?? "ANY",
                Description = r["Description"] == DBNull.Value ? null : r["Description"]?.ToString(),
                Active = Convert.ToBoolean(r["Active"])
            });
    }

    public async Task<List<FulfillmentMethodDTO>> ListCompanyMethodsAsync(int companyId, bool onlyEnabled = true)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@only_enabled", onlyEnabled, SqlDbType.Bit),
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Config.sp_list_company_fulfillment_methods,
            parameters,
            r => new FulfillmentMethodDTO
            {
                Id = Convert.ToInt32(r["Id"]),
                Code = r["Code"]?.ToString() ?? string.Empty,
                Name = r["Name"]?.ToString() ?? string.Empty,
                AppliesTo = r["AppliesTo"]?.ToString() ?? "ANY",
                Description = r["Description"] == DBNull.Value ? null : r["Description"]?.ToString(),
                Active = Convert.ToBoolean(r["Active"]),
                IsEnabledForCompany = Convert.ToBoolean(r["IsEnabled"])
            });
    }

    public async Task<bool> SetCompanyMethodAsync(int companyId, string methodCode, bool enabled)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@method_code", methodCode, SqlDbType.NVarChar, 30),
            CreateParameter("@enabled", enabled, SqlDbType.Bit),
        };

        var ok = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Config.sp_set_company_fulfillment_method,
            parameters,
            r => Convert.ToBoolean(r["Success"])
        );

        return ok == true;
    }
}

