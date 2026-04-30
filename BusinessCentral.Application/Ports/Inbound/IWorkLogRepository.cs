using BusinessCentral.Application.DTOs.Construction;

namespace BusinessCentral.Application.Ports.Outbound;
public interface IWorkLogRepository
{
    Task<int?> InsertWorkLogAsync(int projectId, int? loggedByUserId, DateTime? logDate, string? notes);
    Task InsertWorkLogPhotoAsync(int workLogId, string filePath, string fileName);
    Task<List<WorkLogDto>> ListWorkLogsAsync(int projectId, int page, int pageSize);
}