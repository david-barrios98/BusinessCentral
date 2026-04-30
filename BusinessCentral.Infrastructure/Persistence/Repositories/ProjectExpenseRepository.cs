using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using BusinessCentral.Infrastructure.Constants;

namespace BusinessCentral.Infrastructure.Persistence.Adapters
{
    public class ProjectExpenseRepository : SqlConfigServer, IProjectExpenseRepository
    {
        public ProjectExpenseRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<int?> InsertExpenseAsync(int projectId, decimal amount, string concept, int? spentByUserId)
        {
            var parameters = new[]
            {
                CreateParameter("@ProjectId", projectId, SqlDbType.Int),
                CreateParameter("@Amount", amount, SqlDbType.Decimal),
                CreateParameter("@Concept", concept ?? string.Empty, SqlDbType.VarChar),
                CreateParameter("@SpentByUserId", spentByUserId ?? (object)DBNull.Value, SqlDbType.Int)
            };

            var id = await ExecuteStoredProcedureSingleAsync(
                StoredProcedures.Construction.sp_insert_project_expense,
                parameters,
                reader => Convert.ToInt32(reader.GetValue(0)));

            return id;
        }

        public async Task<List<ProjectExpenseDto>> ListExpensesAsync(int projectId, int page, int pageSize)
        {
            var parameters = new[]
            {
                CreateParameter("@ProjectId", projectId, SqlDbType.Int),
                CreateParameter("@Page", page, SqlDbType.Int),
                CreateParameter("@PageSize", pageSize, SqlDbType.Int)
            };

            return await ExecuteStoredProcedureAsync(
                StoredProcedures.Construction.sp_list_project_expenses,
                parameters,
                reader => SqlDataReaderMapper.MapToDto<ProjectExpenseDto>(reader));
        }
    }
}