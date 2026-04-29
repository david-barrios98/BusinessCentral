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

    public async Task<FulfillmentMethodDTO?> GetMethodByIdAsync(int id)
    {
        var parameters = new[]
        {
            CreateParameter("@Id", id, SqlDbType.Int),
        };

        return await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Config.sp_get_fulfillment_method_by_id,
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

    public async Task<int> UpsertMethodAsync(UpsertFulfillmentMethodRequestDTO request)
    {
        object idParam = request.Id is > 0 ? request.Id.Value : DBNull.Value;

        var parameters = new[]
        {
            CreateParameter("@Id", idParam, SqlDbType.Int),
            CreateParameter("@Code", request.Code.Trim(), SqlDbType.NVarChar, 30),
            CreateParameter("@Name", request.Name.Trim(), SqlDbType.NVarChar, 150),
            CreateParameter("@AppliesTo", request.AppliesTo.Trim(), SqlDbType.NVarChar, 20),
            CreateParameter("@Description", (object?)request.Description ?? DBNull.Value, SqlDbType.NVarChar, 300),
            CreateParameter("@Active", request.Active, SqlDbType.Bit),
        };

        int? newId = await ExecuteStoredProcedureSingleAsync<int>(
            StoredProcedures.Config.sp_upsert_fulfillment_method,
            parameters,
            r => Convert.ToInt32(r["Id"]));

        return newId ?? 0;
    }

    public async Task DeleteMethodAsync(int id)
    {
        var parameters = new[]
        {
            CreateParameter("@Id", id, SqlDbType.Int),
        };

        await ExecuteStoredProcedureNonQueryAsync(
            StoredProcedures.Config.sp_delete_fulfillment_method,
            parameters);
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

