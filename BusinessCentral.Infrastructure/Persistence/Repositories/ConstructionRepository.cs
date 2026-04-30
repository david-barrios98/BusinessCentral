using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Adapters
{
    public class ConstructionRepository : SqlConfigServer, IConstructionRepository
    {
        public ConstructionRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<int?> CreateProjectAsync(CreateProjectDTO dto)
        {
            var parameters = new[]
            {
                CreateParameter("@CompanyId", dto.CompanyId, SqlDbType.Int),
                CreateParameter("@Name", dto.Name, SqlDbType.VarChar),
                CreateParameter("@Description", dto.Description ?? string.Empty, SqlDbType.VarChar),
                CreateParameter("@Location", dto.Location ?? string.Empty, SqlDbType.VarChar),
                CreateParameter("@StartDate", dto.StartDate, SqlDbType.DateTime2),
                CreateParameter("@EndDate", dto.EndDate ?? (object)DBNull.Value, SqlDbType.DateTime2),
                CreateParameter("@Budget", dto.Budget, SqlDbType.Decimal)
            };

            var insertedId = await ExecuteStoredProcedureSingleAsync(
                "[business].[sp_create_construction_project]",
                parameters,
                reader => Convert.ToInt32(reader.GetValue(0)));

            return insertedId;
        }

        public async Task<ProjectResponseDTO?> GetProjectByIdAsync(int projectId)
        {
            var parameters = new[]
            {
                CreateParameter("@ProjectId", projectId, SqlDbType.Int)
            };

            return await ExecuteStoredProcedureSingleAsync(
                "[business].[sp_get_construction_project]",
                parameters,
                reader => SqlDataReaderMapper.MapToDto<ProjectResponseDTO>(reader));
        }

        public async Task UpdateProjectAsync(UpdateProjectDTO dto)
        {
            var parameters = new[]
            {
                CreateParameter("@ProjectId", dto.ProjectId, SqlDbType.Int),
                CreateParameter("@Name", dto.Name, SqlDbType.VarChar),
                CreateParameter("@Description", dto.Description ?? string.Empty, SqlDbType.VarChar),
                CreateParameter("@Location", dto.Location ?? string.Empty, SqlDbType.VarChar),
                CreateParameter("@StartDate", dto.StartDate, SqlDbType.DateTime2),
                CreateParameter("@EndDate", dto.EndDate ?? (object)DBNull.Value, SqlDbType.DateTime2),
                CreateParameter("@Budget", dto.Budget, SqlDbType.Decimal),
                CreateParameter("@Active", dto.Active ? 1 : 0, SqlDbType.Bit)
            };

            await ExecuteStoredProcedureNonQueryAsync("[business].[sp_update_construction_project]", parameters);
        }

        public async Task DeleteProjectAsync(int projectId)
        {
            var parameters = new[]
            {
                CreateParameter("@ProjectId", projectId, SqlDbType.Int)
            };

            await ExecuteStoredProcedureNonQueryAsync("[business].[sp_delete_construction_project]", parameters);
        }

        public async Task<List<ProjectResponseDTO>> ListProjectsAsync(int companyId, int page, int pageSize)
        {
            var parameters = new[]
            {
                CreateParameter("@CompanyId", companyId, SqlDbType.Int),
                CreateParameter("@Page", page, SqlDbType.Int),
                CreateParameter("@PageSize", pageSize, SqlDbType.Int)
            };

            return await ExecuteStoredProcedureAsync(
                "[business].[sp_list_construction_projects]",
                parameters,
                reader => SqlDataReaderMapper.MapToDto<ProjectResponseDTO>(reader));
        }
    }
}