using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class PermissionRepository : SqlConfigServer, IPermissionRepository
{
    public PermissionRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<List<PermissionDTO>> ListPermissionsAsync(bool onlyActive = true, string? moduleCode = null)
    {
        var parameters = new[]
        {
            CreateParameter("@only_active", onlyActive, SqlDbType.Bit),
            CreateParameter("@module_code", (object?)moduleCode ?? DBNull.Value, SqlDbType.NVarChar, 50)
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Config.sp_list_permissions,
            parameters,
            reader => new PermissionDTO
            {
                PermissionId = Convert.ToInt32(reader["PermissionId"]),
                ModuleId = Convert.ToInt32(reader["ModuleId"]),
                ModuleCode = reader["ModuleCode"]?.ToString() ?? string.Empty,
                ModuleName = reader["ModuleName"]?.ToString() ?? string.Empty,
                PermissionCode = reader["PermissionCode"]?.ToString() ?? string.Empty,
                PermissionName = reader["PermissionName"]?.ToString() ?? string.Empty,
                Active = Convert.ToBoolean(reader["Active"])
            });
    }

    public async Task<List<RolePermissionDTO>> ListDefaultPermissionsForNatureAsync(string natureCode)
    {
        var parameters = new[]
        {
            CreateParameter("@nature_code", natureCode, SqlDbType.NVarChar, 50)
        };

        // Si el SP retorna un SELECT de error (Success/Message), esta capa leerá el dataset "bueno".
        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Config.sp_list_business_nature_default_permissions,
            parameters,
            reader => new RolePermissionDTO
            {
                PermissionId = Convert.ToInt32(reader["PermissionId"]),
                ModuleCode = reader["ModuleCode"]?.ToString() ?? string.Empty,
                PermissionCode = reader["PermissionCode"]?.ToString() ?? string.Empty,
                PermissionName = reader["PermissionName"]?.ToString() ?? string.Empty
            });
    }
}

