using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using BusinessCentral.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Adapters
{
    public class ProjectDocumentRepository : SqlConfigServer, IProjectDocumentRepository
    {
        public ProjectDocumentRepository(IConfiguration configuration) : base(configuration) { }

        public async Task InsertProjectDocumentAsync(int projectId, string filePath, string fileName, string documentType, int? uploadedByUserId)
        {
            var parameters = new[]
            {
                CreateParameter("@ProjectId", projectId, SqlDbType.Int),
                CreateParameter("@FilePath", filePath, SqlDbType.VarChar),
                CreateParameter("@FileName", fileName, SqlDbType.VarChar),
                CreateParameter("@DocumentType", documentType, SqlDbType.VarChar),
                CreateParameter("@UploadedByUserId", uploadedByUserId ?? (object)DBNull.Value, SqlDbType.Int)
            };

            await ExecuteStoredProcedureNonQueryAsync(StoredProcedures.Construction.sp_insert_project_document, parameters);
        }

        public async Task<List<ProjectDocumentDto>> ListProjectDocumentsAsync(int projectId)
        {
            var parameters = new[]
            {
                CreateParameter("@ProjectId", projectId, SqlDbType.Int)
            };

            return await ExecuteStoredProcedureAsync(
                StoredProcedures.Construction.sp_list_project_documents,
                parameters,
                reader => SqlDataReaderMapper.MapToDto<ProjectDocumentDto>(reader));
        }
    }
}