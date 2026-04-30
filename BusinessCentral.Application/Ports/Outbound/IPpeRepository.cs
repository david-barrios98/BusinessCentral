using BusinessCentral.Application.DTOs.Construction;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface IPpeRepository
    {
        Task<int?> InsertPpeRecordAsync(int? projectId, int userId, string item, string? notes);
        Task<List<PpeDto>> ListPpeAsync(int projectId);
    }
}