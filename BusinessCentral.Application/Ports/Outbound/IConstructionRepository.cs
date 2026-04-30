using BusinessCentral.Application.DTOs.Construction;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface IConstructionRepository
    {
        Task<int?> CreateProjectAsync(CreateProjectDTO dto);
        Task<ProjectResponseDTO?> GetProjectByIdAsync(int projectId);
        Task UpdateProjectAsync(UpdateProjectDTO dto);
        Task DeleteProjectAsync(int projectId);
        Task<List<ProjectResponseDTO>> ListProjectsAsync(int companyId, int page, int pageSize);
    }
}