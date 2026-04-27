using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class PublicAccessRepository : SqlConfigServer, IPublicAccessRepository
{
    public PublicAccessRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<long> CreatePublicTokenAsync(int companyId, int userId, string tokenHashHex, string scope, DateTime expiresAtUtc)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@user_id", userId, SqlDbType.Int),
            CreateParameter("@token_hash", tokenHashHex, SqlDbType.NVarChar, 64),
            CreateParameter("@scope", scope, SqlDbType.NVarChar, 30),
            CreateParameter("@expires_at", expiresAtUtc, SqlDbType.DateTime2),
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Auth.sp_create_public_access_token,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return insertedId;
    }

    public async Task<PublicHrAccountSummaryDTO> GetHrAccountSummaryByTokenAsync(string tokenHashHex)
    {
        var parameters = new[]
        {
            CreateParameter("@token_hash", tokenHashHex, SqlDbType.NVarChar, 64),
        };

        var data = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Auth.sp_get_public_hr_account_summary,
            parameters,
            r => new PublicHrAccountSummaryDTO
            {
                CompanyId = Convert.ToInt32(r["CompanyId"]),
                UserId = Convert.ToInt32(r["UserId"]),
                TotalEarned = Convert.ToDecimal(r["TotalEarned"]),
                TotalDeductions = Convert.ToDecimal(r["TotalDeductions"]),
                TotalLoans = Convert.ToDecimal(r["TotalLoans"]),
                Net = Convert.ToDecimal(r["Net"]),
            }
        );

        return data ?? new PublicHrAccountSummaryDTO();
    }
}

