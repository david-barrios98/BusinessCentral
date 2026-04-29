using BusinessCentral.Application.DTOs.Config;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class BusinessNatureRepository : SqlConfigServer, IBusinessNatureRepository
{
    public BusinessNatureRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<List<BusinessNatureDTO>> ListAsync(bool onlyActive = true)
    {
        var parameters = new[]
        {
            CreateParameter("@only_active", onlyActive, SqlDbType.Bit)
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Config.sp_list_business_natures,
            parameters,
            r => new BusinessNatureDTO
            {
                Id = Convert.ToInt32(r["Id"]),
                Code = r["Code"]?.ToString() ?? string.Empty,
                Name = r["Name"]?.ToString() ?? string.Empty,
                Description = r["Description"] == DBNull.Value ? null : r["Description"]?.ToString(),
                Active = Convert.ToBoolean(r["Active"])
            });
    }

    public async Task<BusinessNatureDTO?> GetByIdAsync(int id)
    {
        var parameters = new[]
        {
            CreateParameter("@Id", id, SqlDbType.Int)
        };

        return await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Config.sp_get_business_nature_by_id,
            parameters,
            r => new BusinessNatureDTO
            {
                Id = Convert.ToInt32(r["Id"]),
                Code = r["Code"]?.ToString() ?? string.Empty,
                Name = r["Name"]?.ToString() ?? string.Empty,
                Description = r["Description"] == DBNull.Value ? null : r["Description"]?.ToString(),
                Active = Convert.ToBoolean(r["Active"])
            });
    }

    public async Task<int> UpsertAsync(UpsertBusinessNatureRequestDTO request)
    {
        object idParam = request.Id is > 0 ? request.Id.Value : DBNull.Value;

        var parameters = new[]
        {
            CreateParameter("@Id", idParam, SqlDbType.Int),
            CreateParameter("@Code", request.Code.Trim(), SqlDbType.NVarChar, 50),
            CreateParameter("@Name", request.Name.Trim(), SqlDbType.NVarChar, 150),
            CreateParameter("@Description", (object?)request.Description ?? DBNull.Value, SqlDbType.NVarChar, 500),
            CreateParameter("@Active", request.Active, SqlDbType.Bit),
        };

        int? newId = await ExecuteStoredProcedureSingleAsync<int>(
            StoredProcedures.Config.sp_upsert_business_nature,
            parameters,
            r => Convert.ToInt32(r["Id"]));

        return newId ?? 0;
    }

    public async Task DeleteAsync(int id)
    {
        var parameters = new[]
        {
            CreateParameter("@Id", id, SqlDbType.Int)
        };

        await ExecuteStoredProcedureNonQueryAsync(
            StoredProcedures.Config.sp_delete_business_nature,
            parameters);
    }
}

