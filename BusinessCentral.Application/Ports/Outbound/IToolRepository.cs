using BusinessCentral.Application.DTOs.Construction;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface IToolRepository
    {
        Task<int?> CreateToolAsync(int companyId, string code, string name, string? serialNumber);
        Task LoanToolAsync(int toolId, int? projectId, int? borrowedByUserId, string? notes);
        Task<List<ToolDto>> ListToolsAsync(int companyId);
        Task<List<ToolLoanDto>> ListToolLoansAsync(int toolId);
    }
}