using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BusinessCentral.Infrastructure.Persistence.Adapters
{
    public class ToolRepository : SqlConfigServer, IToolRepository
    {
        public ToolRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<int?> CreateToolAsync(int companyId, string code, string name, string? serialNumber)
        {
            var parameters = new[]
            {
                CreateParameter("@CompanyId", companyId, SqlDbType.Int),
                CreateParameter("@Code", code ?? string.Empty, SqlDbType.VarChar),
                CreateParameter("@Name", name ?? string.Empty, SqlDbType.VarChar),
                CreateParameter("@SerialNumber", serialNumber ?? string.Empty, SqlDbType.VarChar)
            };

            var id = await ExecuteStoredProcedureSingleAsync(
                "[construction].[sp_create_tool]",
                parameters,
                reader => Convert.ToInt32(reader.GetValue(0)));

            return id;
        }

        public async Task LoanToolAsync(int toolId, int? projectId, int? borrowedByUserId, string? notes)
        {
            var parameters = new[]
            {
                CreateParameter("@ToolId", toolId, SqlDbType.Int),
                CreateParameter("@ProjectId", projectId ?? (object)DBNull.Value, SqlDbType.Int),
                CreateParameter("@BorrowedByUserId", borrowedByUserId ?? (object)DBNull.Value, SqlDbType.Int),
                CreateParameter("@Notes", notes ?? string.Empty, SqlDbType.VarChar)
            };

            await ExecuteStoredProcedureNonQueryAsync("[construction].[sp_loan_tool]", parameters);
        }

        public async Task<List<ToolDto>> ListToolsAsync(int companyId)
        {
            var parameters = new[]
            {
                CreateParameter("@CompanyId", companyId, SqlDbType.Int)
            };

            return await ExecuteStoredProcedureAsync(
                "[construction].[sp_list_tools]",
                parameters,
                reader => SqlDataReaderMapper.MapToDto<ToolDto>(reader));
        }

        public async Task<List<ToolLoanDto>> ListToolLoansAsync(int toolId)
        {
            var parameters = new[]
            {
                CreateParameter("@ToolId", toolId, SqlDbType.Int)
            };

            return await ExecuteStoredProcedureAsync(
                "[construction].[sp_list_tool_loans]",
                parameters,
                reader => SqlDataReaderMapper.MapToDto<ToolLoanDto>(reader));
        }
    }
}