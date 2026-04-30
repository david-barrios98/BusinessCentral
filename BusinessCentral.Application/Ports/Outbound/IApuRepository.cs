using BusinessCentral.Application.DTOs.Construction;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface IApuRepository
    {
        Task<int?> InsertApuItemAsync(int projectId, string? code, string? description, string unit, decimal unitPrice, decimal yield);
        Task<List<ApuItemDto>> ListApuItemsAsync(int projectId);
    }
}