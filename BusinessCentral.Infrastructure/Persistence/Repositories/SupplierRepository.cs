using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;

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

    public async Task<List<SupplierDTO>> ListAsync(int companyId, bool onlyActive = true, string? q = null)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@only_active", onlyActive, SqlDbType.Bit),
            CreateParameter("@q", (object?)q ?? DBNull.Value, SqlDbType.NVarChar, 100)
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Commerce.sp_list_suppliers,
            parameters,
            r => new SupplierDTO
            {
                Id = Convert.ToInt64(r["Id"]),
                CompanyId = Convert.ToInt32(r["CompanyId"]),
                Name = r["Name"]?.ToString() ?? string.Empty,
                DocumentNumber = r["DocumentNumber"] == DBNull.Value ? null : r["DocumentNumber"]?.ToString(),
                Phone = r["Phone"] == DBNull.Value ? null : r["Phone"]?.ToString(),
                Email = r["Email"] == DBNull.Value ? null : r["Email"]?.ToString(),
                Notes = r["Notes"] == DBNull.Value ? null : r["Notes"]?.ToString(),
                Active = Convert.ToBoolean(r["Active"])
            });
    }
}

