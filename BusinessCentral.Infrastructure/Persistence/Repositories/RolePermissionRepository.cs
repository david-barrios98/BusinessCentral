using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class RolePermissionRepository : SqlConfigServer, IRolePermissionRepository
{
    public RolePermissionRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<List<RolePermissionDTO>> ListRolePermissionsAsync(int roleId)
    {
        var parameters = new[]
        {
            CreateParameter("@role_id", roleId, SqlDbType.Int)
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Config.sp_list_role_permissions,
            parameters,
            reader => new RolePermissionDTO
            {
                PermissionId = Convert.ToInt32(reader["PermissionId"]),
                ModuleCode = reader["ModuleCode"]?.ToString() ?? string.Empty,
                PermissionCode = reader["PermissionCode"]?.ToString() ?? string.Empty,
                PermissionName = reader["PermissionName"]?.ToString() ?? string.Empty
            });
    }

    public async Task<bool> SetRolePermissionAsync(int roleId, int permissionId, bool enabled)
    {
        var parameters = new[]
        {
            CreateParameter("@role_id", roleId, SqlDbType.Int),
            CreateParameter("@permission_id", permissionId, SqlDbType.Int),
            CreateParameter("@enabled", enabled, SqlDbType.Bit)
        };

        var ok = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Config.sp_set_role_permission,
            parameters,
            reader => Convert.ToBoolean(reader["Success"])
        );

        return ok == true;
    }
}

