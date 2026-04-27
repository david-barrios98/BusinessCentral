using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class TenantRepository : SqlConfigServer, ITenantRepository
{
    public TenantRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<int?> GetCompanyIdBySubdomainAsync(string subdomain)
    {
        try
        {
            var parameters = new[]
            {
                CreateParameter("@subdomain", subdomain, SqlDbType.NVarChar, 50),
            };

            return await ExecuteStoredProcedureSingleAsync(
                StoredProcedures.Config.sp_get_company_id_by_subdomain,
                parameters,
                reader => reader["CompanyId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["CompanyId"])
            );
        }
        catch
        {
            // Si el SP no existe aún o hay falla de DB, no bloqueamos la app.
            return null;
        }
    }
}

