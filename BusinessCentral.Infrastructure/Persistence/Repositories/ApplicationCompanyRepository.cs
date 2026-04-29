using BusinessCentral.Application.DTOs.Config;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class ApplicationCompanyRepository : SqlConfigServer, IApplicationCompanyRepository
{
    public ApplicationCompanyRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<List<ApplicationCompanyDTO>> ListByCompanyAsync(int companyId)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Config.sp_list_application_companies,
            parameters,
            ReadRow);
    }

    public async Task<int> UpsertAsync(int companyId, UpsertApplicationCompanyRequestDTO request)
    {
        var loginField = NormalizeLoginField(request.LoginField);
        var idParam = request.Id is > 0 ? request.Id.Value : (object)DBNull.Value;

        var parameters = new[]
        {
            CreateParameter("@Id", idParam, SqlDbType.Int),
            CreateParameter("@CompanyId", companyId, SqlDbType.Int),
            CreateParameter("@ApplicationCode", request.ApplicationCode.Trim(), SqlDbType.NVarChar, 200),
            CreateParameter("@LoginField", loginField, SqlDbType.NVarChar, 20),
            CreateParameter("@Priority", request.Priority, SqlDbType.Int),
            CreateParameter("@IsEnabled", request.IsEnabled, SqlDbType.Bit),
            CreateParameter("@Active", request.Active, SqlDbType.Bit),
        };

        int? newId = await ExecuteStoredProcedureSingleAsync<int>(
            StoredProcedures.Config.sp_upsert_application_company,
            parameters,
            r => Convert.ToInt32(r["Id"]));

        return newId ?? 0;
    }

    public async Task DeleteAsync(int companyId, int id)
    {
        var parameters = new[]
        {
            CreateParameter("@Id", id, SqlDbType.Int),
            CreateParameter("@CompanyId", companyId, SqlDbType.Int),
        };

        await ExecuteStoredProcedureNonQueryAsync(
            StoredProcedures.Config.sp_delete_application_company,
            parameters);
    }

    private static string NormalizeLoginField(string raw)
    {
        var v = raw.Trim().ToLowerInvariant();
        return v == "documentnumber" ? "document" : v;
    }

    private static ApplicationCompanyDTO ReadRow(SqlDataReader r)
    {
        return new ApplicationCompanyDTO
        {
            Id = Convert.ToInt32(r["Id"]),
            CompanyId = Convert.ToInt32(r["CompanyId"]),
            ApplicationCode = r["ApplicationCode"]?.ToString() ?? string.Empty,
            LoginField = r["LoginField"]?.ToString() ?? string.Empty,
            Priority = Convert.ToInt32(r["Priority"]),
            IsEnabled = Convert.ToBoolean(r["IsEnabled"]),
            Create = r["Create"] == DBNull.Value ? null : Convert.ToDateTime(r["Create"]),
            Update = r["Update"] == DBNull.Value ? null : Convert.ToDateTime(r["Update"]),
            Active = r["Active"] == DBNull.Value ? null : Convert.ToBoolean(r["Active"]),
        };
    }
}
