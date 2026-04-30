using BusinessCentral.Application.DTOs.Construction;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface IProjectExpenseRepository
    {
        Task<int?> InsertExpenseAsync(int projectId, decimal amount, string concept, int? spentByUserId);
        Task<List<ProjectExpenseDto>> ListExpensesAsync(int projectId, int page, int pageSize);
    }
}