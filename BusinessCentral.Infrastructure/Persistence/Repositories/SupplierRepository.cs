using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class SupplierRepository : SqlConfigServer, ISupplierRepository
{
    public SupplierRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<long> UpsertAsync(int companyId, long? id, string name, string? documentNumber, string? phone, string? email, string? notes, bool active)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@id", (object?)id ?? DBNull.Value, SqlDbType.BigInt),
            CreateParameter("@name", name, SqlDbType.NVarChar, 200),
            CreateParameter("@document_number", (object?)documentNumber ?? DBNull.Value, SqlDbType.NVarChar, 50),
            CreateParameter("@phone", (object?)phone ?? DBNull.Value, SqlDbType.NVarChar, 20),
            CreateParameter("@email", (object?)email ?? DBNull.Value, SqlDbType.NVarChar, 150),
            CreateParameter("@notes", (object?)notes ?? DBNull.Value, SqlDbType.NVarChar, 500),
            CreateParameter("@active", active, SqlDbType.Bit)
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Commerce.sp_upsert_supplier,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return insertedId;
    }

    public async Task<PagedResult<SupplierDTO>> ListAsync(int companyId, bool onlyActive = true, int page = 1, int pageSize = 50, string? q = null)
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(StoredProcedures.Commerce.sp_list_suppliers, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(CreateParameter("@company_id", companyId, SqlDbType.Int));
        command.Parameters.Add(CreateParameter("@only_active", onlyActive, SqlDbType.Bit));
        command.Parameters.Add(CreateParameter("@q", (object?)q ?? DBNull.Value, SqlDbType.NVarChar, 100));
        command.Parameters.Add(CreateParameter("@page", page, SqlDbType.Int));
        command.Parameters.Add(CreateParameter("@page_size", pageSize, SqlDbType.Int));

        var items = new List<SupplierDTO>();
        long total = 0;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(new SupplierDTO
            {
                Id = Convert.ToInt64(reader["Id"]),
                CompanyId = Convert.ToInt32(reader["CompanyId"]),
                Name = reader["Name"]?.ToString() ?? string.Empty,
                DocumentNumber = reader["DocumentNumber"] == DBNull.Value ? null : reader["DocumentNumber"]?.ToString(),
                Phone = reader["Phone"] == DBNull.Value ? null : reader["Phone"]?.ToString(),
                Email = reader["Email"] == DBNull.Value ? null : reader["Email"]?.ToString(),
                Notes = reader["Notes"] == DBNull.Value ? null : reader["Notes"]?.ToString(),
                Active = Convert.ToBoolean(reader["Active"])
            });
        }

        if (await reader.NextResultAsync() && await reader.ReadAsync())
            total = Convert.ToInt64(reader["Total"]);

        return new PagedResult<SupplierDTO>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }
}

