using BusinessCentral.Application.Constants;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Persistence.Adapters;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories
{
    public class SubscriptionRepository : SqlConfigServer, ISubscriptionRepository
    {
        private readonly BusinessCentralDbContext _context;

        public SubscriptionRepository(IConfiguration configuration, BusinessCentralDbContext context) : base(configuration)
        {
            _context = context;
        }
        public async Task<AccessResult> CheckAccessAsync(int companyId, string? moduleName)
        {
            var parameters = new[]
            {
                CreateParameter("@company_id", companyId, SqlDbType.Int),
                CreateParameter("@module_name", (object)moduleName ?? DBNull.Value, SqlDbType.VarChar)
            };

            // Ejecutamos el SP y mapeamos el string resultante al Enum AccessResult
            var result = await ExecuteStoredProcedureSingleAsync(
                "auth.sp_check_tenant_access",
                parameters,
                reader => reader["AccessStatus"].ToString()
            );

            // Convertimos el string del SP al Enum de C#
            if (Enum.TryParse<AccessResult>(result, out var accessResult))
            {
                return accessResult;
            }

            return AccessResult.CompanyDisabled; // Por defecto, si algo falla, bloqueamos
        }
    }
}
