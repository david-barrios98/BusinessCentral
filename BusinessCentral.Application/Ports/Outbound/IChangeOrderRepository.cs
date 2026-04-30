using BusinessCentral.Application.DTOs.Construction;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface IChangeOrderRepository
    {
        Task<int?> CreateChangeOrderAsync(int projectId, string description, decimal amount, int? requestedByUserId);
        Task<List<ChangeOrderDto>> ListChangeOrdersAsync(int projectId);
    }
}