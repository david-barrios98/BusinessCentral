using BusinessCentral.Application.DTOs.Business;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class StorageLocationRepository : SqlConfigServer, IStorageLocationRepository
{
    public StorageLocationRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<long> UpsertAsync(int companyId, long? id, int? facilityId, string code, string name, string type, long? parentLocationId, string? notes, bool active)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@id", (object?)id ?? DBNull.Value, SqlDbType.BigInt),
            CreateParameter("@facility_id", (object?)facilityId ?? DBNull.Value, SqlDbType.Int),
            CreateParameter("@code", code, SqlDbType.NVarChar, 50),
            CreateParameter("@name", name, SqlDbType.NVarChar, 200),
            CreateParameter("@type", type, SqlDbType.NVarChar, 30),
            CreateParameter("@parent_location_id", (object?)parentLocationId ?? DBNull.Value, SqlDbType.BigInt),
            CreateParameter("@notes", (object?)notes ?? DBNull.Value, SqlDbType.NVarChar, 500),
            CreateParameter("@active", active, SqlDbType.Bit)
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Business.sp_upsert_storage_location,
            parameters,
            reader => Convert.ToInt64(reader["InsertedId"])
        );

        return insertedId;
    }

    public async Task<List<StorageLocationDTO>> ListAsync(int companyId, int? facilityId = null, bool onlyActive = true)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@facility_id", (object?)facilityId ?? DBNull.Value, SqlDbType.Int),
            CreateParameter("@only_active", onlyActive, SqlDbType.Bit)
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Business.sp_list_storage_locations,
            parameters,
            reader => new StorageLocationDTO
            {
                Id = Convert.ToInt64(reader["Id"]),
                CompanyId = Convert.ToInt32(reader["CompanyId"]),
                FacilityId = reader["FacilityId"] == DBNull.Value ? null : Convert.ToInt32(reader["FacilityId"]),
                Code = reader["Code"]?.ToString() ?? string.Empty,
                Name = reader["Name"]?.ToString() ?? string.Empty,
                Type = reader["Type"]?.ToString() ?? "WAREHOUSE",
                ParentLocationId = reader["ParentLocationId"] == DBNull.Value ? null : Convert.ToInt64(reader["ParentLocationId"]),
                Notes = reader["Notes"] == DBNull.Value ? null : reader["Notes"]?.ToString(),
                Active = Convert.ToBoolean(reader["Active"]),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
            });
    }
}

