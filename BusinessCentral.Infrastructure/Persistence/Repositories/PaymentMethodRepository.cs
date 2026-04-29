using BusinessCentral.Application.DTOs.Config;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class PaymentMethodRepository : SqlConfigServer, IPaymentMethodRepository
{
    public PaymentMethodRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<List<PaymentMethodDTO>> ListMethodsAsync(bool onlyActive = true)
    {
        var parameters = new[]
        {
            CreateParameter("@only_active", onlyActive, SqlDbType.Bit),
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Config.sp_list_payment_methods,
            parameters,
            r => new PaymentMethodDTO
            {
                Id = Convert.ToInt32(r["Id"]),
                Code = r["Code"]?.ToString() ?? string.Empty,
                Name = r["Name"]?.ToString() ?? string.Empty,
                AppliesTo = r["AppliesTo"]?.ToString() ?? "ANY",
                Description = r["Description"] == DBNull.Value ? null : r["Description"]?.ToString(),
                Active = Convert.ToBoolean(r["Active"])
            });
    }

    public async Task<PaymentMethodDTO?> GetMethodByIdAsync(int id)
    {
        var parameters = new[]
        {
            CreateParameter("@Id", id, SqlDbType.Int),
        };

        return await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Config.sp_get_payment_method_by_id,
            parameters,
            r => new PaymentMethodDTO
            {
                Id = Convert.ToInt32(r["Id"]),
                Code = r["Code"]?.ToString() ?? string.Empty,
                Name = r["Name"]?.ToString() ?? string.Empty,
                AppliesTo = r["AppliesTo"]?.ToString() ?? "ANY",
                Description = r["Description"] == DBNull.Value ? null : r["Description"]?.ToString(),
                Active = Convert.ToBoolean(r["Active"])
            });
    }

    public async Task<int> UpsertMethodAsync(UpsertPaymentMethodRequestDTO request)
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
            StoredProcedures.Config.sp_upsert_payment_method,
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
            StoredProcedures.Config.sp_delete_payment_method,
            parameters);
    }

    public async Task<List<PaymentMethodDTO>> ListCompanyMethodsAsync(int companyId, bool onlyEnabled = true)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@only_enabled", onlyEnabled, SqlDbType.Bit),
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Config.sp_list_company_payment_methods,
            parameters,
            r => new PaymentMethodDTO
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
            StoredProcedures.Config.sp_set_company_payment_method,
            parameters,
            r => Convert.ToBoolean(r["Success"])
        );

        return ok == true;
    }
}

