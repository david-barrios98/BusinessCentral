using BusinessCentral.Application.DTOs.Hr;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IHrRepository
{
    Task<bool> UpsertEmployeeProfileAsync(int companyId, EmployeeProfileDTO dto);
    Task<EmployeeProfileDTO?> GetEmployeeProfileAsync(int companyId, int userId);
    Task<List<dynamic>> ListEmployeesAsync(int companyId);
    Task<bool> UpsertEmployeeAvailabilityAsync(int companyId, EmployeeAvailabilityDTO dto);
    Task<EmployeeAvailabilityDTO?> GetEmployeeAvailabilityAsync(int companyId, int userId);

    Task<bool> UpsertPaySchemeAsync(int companyId, string code, string name, string? unit, bool active);
    Task<List<PaySchemeDTO>> ListPaySchemesAsync(int companyId, bool onlyActive);

    Task<long> CreateWorkLogAsync(int companyId, WorkLogDTO dto);
    Task<List<WorkLogDTO>> ListWorkLogsAsync(int companyId, int? userId, DateTime? fromDate, DateTime? toDate);

    Task<long> CreateLoanAdvanceAsync(int companyId, LoanAdvanceDTO dto);
    Task<List<LoanAdvanceDTO>> ListLoanAdvancesAsync(int companyId, int? userId, string? status);

    Task<long> CreateDeductionAsync(int companyId, DeductionDTO dto);
    Task<List<DeductionDTO>> ListDeductionsAsync(int companyId, int? userId, string? type);
}

