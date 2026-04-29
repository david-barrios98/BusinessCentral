using BusinessCentral.Application.DTOs.Config;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class FacilityTypeRepository : SqlConfigServer, IFacilityTypeRepository
{
    public FacilityTypeRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<List<FacilityTypeDTO>> ListAsync(bool onlyActive = true)
    {
        var parameters = new[]
        {
            CreateParameter("@only_active", onlyActive, SqlDbType.Bit)
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Business.sp_list_facility_types,
            parameters,
            r => new FacilityTypeDTO
            {
                Id = Convert.ToInt32(r["Id"]),
                Name = r["Name"]?.ToString() ?? string.Empty,
                Active = Convert.ToBoolean(r["Active"]),
                Create = Convert.ToDateTime(r["Create"]),
                Update = Convert.ToDateTime(r["Update"])
            });
    }

    public async Task<FacilityTypeDTO?> GetByIdAsync(int id)
    {
        var parameters = new[]
        {
            CreateParameter("@Id", id, SqlDbType.Int)
        };

        return await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Business.sp_get_facility_type_by_id,
            parameters,
            r => new FacilityTypeDTO
            {
                Id = Convert.ToInt32(r["Id"]),
                Name = r["Name"]?.ToString() ?? string.Empty,
                Active = Convert.ToBoolean(r["Active"]),
                Create = Convert.ToDateTime(r["Create"]),
                Update = Convert.ToDateTime(r["Update"])
            });
    }

    public async Task<int> UpsertAsync(UpsertFacilityTypeRequestDTO request)
    {
        object idParam = request.Id is > 0 ? request.Id.Value : DBNull.Value;

        var parameters = new[]
        {
            CreateParameter("@Id", idParam, SqlDbType.Int),
            CreateParameter("@Name", request.Name.Trim(), SqlDbType.NVarChar, 200),
            CreateParameter("@Active", request.Active, SqlDbType.Bit)
        };

        int? newId = await ExecuteStoredProcedureSingleAsync<int>(
            StoredProcedures.Business.sp_upsert_facility_type,
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
            StoredProcedures.Business.sp_delete_facility_type,
            parameters);
    }
}

