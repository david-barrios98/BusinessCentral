using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace BusinessCentral.Infrastructure.Persistence.Adapters;
public class WorkLogRepository : SqlConfigServer, IWorkLogRepository
{
    public WorkLogRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<int?> InsertWorkLogAsync(int projectId, int? loggedByUserId, DateTime? logDate, string? notes)
    {
        var parameters = new[]
        {
            CreateParameter("@ProjectId", projectId, SqlDbType.Int),
            CreateParameter("@LoggedByUserId", loggedByUserId ?? (object)DBNull.Value, SqlDbType.Int),
            CreateParameter("@LogDate", logDate ?? (object)DBNull.Value, SqlDbType.DateTime2),
            CreateParameter("@Notes", notes ?? string.Empty, SqlDbType.VarChar),
        };

        var id = await ExecuteStoredProcedureSingleAsync("[construction].[sp_insert_worklog]", parameters, reader => Convert.ToInt32(reader.GetValue(0)));
        return id;
    }

    public async Task InsertWorkLogPhotoAsync(int workLogId, string filePath, string fileName)
    {
        var parameters = new[]
        {
            CreateParameter("@WorkLogId", workLogId, SqlDbType.Int),
            CreateParameter("@FilePath", filePath, SqlDbType.VarChar),
            CreateParameter("@FileName", fileName, SqlDbType.VarChar)
        };
        await ExecuteStoredProcedureNonQueryAsync("[construction].[sp_insert_worklog_photo]", parameters);
    }

    public async Task<List<WorkLogDto>> ListWorkLogsAsync(int projectId, int page, int pageSize)
    {
        // Implementa SP de lista si lo deseas. Aquí ejemplo con EF (rápido) o crear sp_list_worklogs.
        var parameters = new[]
        {
            CreateParameter("@ProjectId", projectId, SqlDbType.Int),
            CreateParameter("@Page", page, SqlDbType.Int),
            CreateParameter("@PageSize", pageSize, SqlDbType.Int)
        };

        // Asume que existe "[construction].[sp_list_worklogs]" mapeando a WorkLogDto
        return await ExecuteStoredProcedureAsync("[construction].[sp_list_worklogs]", parameters, reader => SqlDataReaderMapper.MapToDto<WorkLogDto>(reader));
    }
}