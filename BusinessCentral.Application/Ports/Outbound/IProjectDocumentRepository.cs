using BusinessCentral.Application.DTOs.Construction;

namespace BusinessCentral.Application.Ports.Outbound
{
    public interface IProjectDocumentRepository
    {
        Task InsertProjectDocumentAsync(int projectId, string filePath, string fileName, string documentType, int? uploadedByUserId);
        Task<List<ProjectDocumentDto>> ListProjectDocumentsAsync(int projectId);
    }
}