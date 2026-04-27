using BusinessCentral.Application.DTOs.Business;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class StorageLocationRepository : SqlConfigServer, IStorageLocationRepository
{
    public StorageLocationRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<long> UpsertAsync(int companyId, long? id, int? facilityId, string code, string name, string type, long? parentLocationId, string? notes, bool active, int? performedByUserId)
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
            CreateParameter("@active", active, SqlDbType.Bit),
            CreateParameter("@performed_by_user_id", (object?)performedByUserId ?? DBNull.Value, SqlDbType.Int),
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Business.sp_upsert_storage_location,
            parameters,
            reader => Convert.ToInt64(reader["InsertedId"])
        );

        return insertedId;
    }

    public async Task<PagedResult<StorageLocationDTO>> ListAsync(int companyId, int? facilityId = null, bool onlyActive = true, int page = 1, int pageSize = 50)
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(StoredProcedures.Business.sp_list_storage_locations, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(CreateParameter("@company_id", companyId, SqlDbType.Int));
        command.Parameters.Add(CreateParameter("@facility_id", (object?)facilityId ?? DBNull.Value, SqlDbType.Int));
        command.Parameters.Add(CreateParameter("@only_active", onlyActive, SqlDbType.Bit));
        command.Parameters.Add(CreateParameter("@page", page, SqlDbType.Int));
        command.Parameters.Add(CreateParameter("@page_size", pageSize, SqlDbType.Int));

        var items = new List<StorageLocationDTO>();
        long total = 0;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(new StorageLocationDTO
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

        if (await reader.NextResultAsync() && await reader.ReadAsync())
            total = Convert.ToInt64(reader["Total"]);

        return new PagedResult<StorageLocationDTO>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }
}

