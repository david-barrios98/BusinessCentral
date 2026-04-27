using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text.Json;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class HrRepository : SqlConfigServer, IHrRepository
{
    public HrRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<bool> UpsertEmployeeProfileAsync(int companyId, EmployeeProfileDTO dto)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@user_id", dto.UserId, SqlDbType.Int),
            CreateParameter("@is_employee", dto.IsEmployee, SqlDbType.Bit),
            CreateParameter("@active_employee", dto.ActiveEmployee, SqlDbType.Bit),
            CreateParameter("@hire_date", (object?)dto.HireDate?.Date ?? DBNull.Value, SqlDbType.Date),
            CreateParameter("@lodging_included", dto.LodgingIncluded, SqlDbType.Bit),
            CreateParameter("@lodging_location", (object?)dto.LodgingLocation ?? DBNull.Value, SqlDbType.NVarChar, 200),
            CreateParameter("@mattress_included", dto.MattressIncluded, SqlDbType.Bit),
            CreateParameter("@meal_plan_code", (object?)dto.MealPlanCode ?? DBNull.Value, SqlDbType.NVarChar, 50),
            CreateParameter("@meal_unit_cost", (object?)dto.MealUnitCost ?? DBNull.Value, SqlDbType.Decimal),
        };

        var success = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Hr.sp_upsert_employee_profile,
            parameters,
            r => Convert.ToBoolean(r["Success"])
        );

        return success == true;
    }

    public async Task<EmployeeProfileDTO?> GetEmployeeProfileAsync(int companyId, int userId)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@user_id", userId, SqlDbType.Int),
        };

        return await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Hr.sp_get_employee_profile,
            parameters,
            r => new EmployeeProfileDTO
            {
                UserId = Convert.ToInt32(r["UserId"]),
                CompanyId = Convert.ToInt32(r["CompanyId"]),
                IsEmployee = Convert.ToBoolean(r["IsEmployee"]),
                ActiveEmployee = Convert.ToBoolean(r["ActiveEmployee"]),
                HireDate = r["HireDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(r["HireDate"]),
                LodgingIncluded = Convert.ToBoolean(r["LodgingIncluded"]),
                LodgingLocation = r["LodgingLocation"] == DBNull.Value ? null : r["LodgingLocation"].ToString(),
                MattressIncluded = Convert.ToBoolean(r["MattressIncluded"]),
                MealPlanCode = r["MealPlanCode"] == DBNull.Value ? null : r["MealPlanCode"].ToString(),
                MealUnitCost = r["MealUnitCost"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(r["MealUnitCost"]),
            }
        );
    }

    public async Task<List<dynamic>> ListEmployeesAsync(int companyId)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int)
        };

        // Para no forzar un DTO grande en esta primera etapa, devolvemos dinámico con lo mínimo.
        var list = await ExecuteStoredProcedureAsync(
            StoredProcedures.Hr.sp_list_employees,
            parameters,
            r => (dynamic)new
            {
                UserId = Convert.ToInt32(r["UserId"]),
                CompanyId = Convert.ToInt32(r["CompanyId"]),
                FirstName = r["FirstName"]?.ToString(),
                LastName = r["LastName"]?.ToString(),
                DocumentNumber = r["DocumentNumber"]?.ToString(),
                Phone = r["Phone"]?.ToString(),
                Email = r["Email"]?.ToString(),
                RoleId = Convert.ToInt32(r["RoleId"]),
                UserActive = Convert.ToBoolean(r["UserActive"]),
                ActiveEmployee = r["ActiveEmployee"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(r["ActiveEmployee"]),
                HireDate = r["HireDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["HireDate"]),
            }
        );

        return list;
    }

    public async Task<bool> UpsertEmployeeAvailabilityAsync(int companyId, EmployeeAvailabilityDTO dto)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@user_id", dto.UserId, SqlDbType.Int),
            CreateParameter("@time_zone", (object?)dto.TimeZone ?? DBNull.Value, SqlDbType.NVarChar, 80),
            CreateParameter("@max_services_per_day", (object?)dto.MaxServicesPerDay ?? DBNull.Value, SqlDbType.Int),
            CreateParameter("@active", dto.Active, SqlDbType.Bit),
            CreateParameter("@notes", (object?)dto.Notes ?? DBNull.Value, SqlDbType.NVarChar, 500),
        };

        var success = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Hr.sp_upsert_employee_availability,
            parameters,
            r => Convert.ToBoolean(r["Success"])
        );

        if (success != true)
            return false;

        var slotsJson = JsonSerializer.Serialize(dto.Slots ?? new List<EmployeeAvailabilitySlotDTO>());
        var exceptionsJson = JsonSerializer.Serialize(dto.Exceptions ?? new List<EmployeeAvailabilityExceptionDTO>());

        var slotsOk = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Hr.sp_set_employee_availability_slots,
            new[]
            {
                CreateParameter("@company_id", companyId, SqlDbType.Int),
                CreateParameter("@user_id", dto.UserId, SqlDbType.Int),
                CreateParameter("@slots_json", slotsJson, SqlDbType.NVarChar),
            },
            r => Convert.ToBoolean(r["Success"])
        );

        if (slotsOk != true)
            return false;

        var excOk = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Hr.sp_set_employee_availability_exceptions,
            new[]
            {
                CreateParameter("@company_id", companyId, SqlDbType.Int),
                CreateParameter("@user_id", dto.UserId, SqlDbType.Int),
                CreateParameter("@exceptions_json", exceptionsJson, SqlDbType.NVarChar),
            },
            r => Convert.ToBoolean(r["Success"])
        );

        return excOk == true;
    }

    public async Task<EmployeeAvailabilityDTO?> GetEmployeeAvailabilityAsync(int companyId, int userId)
    {
        using var connection = await OpenConnectionAsync();
        using var command = new SqlCommand(StoredProcedures.Hr.sp_get_employee_availability, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddRange(new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@user_id", userId, SqlDbType.Int),
        });

        using var reader = await command.ExecuteReaderAsync();
        EmployeeAvailabilityDTO? dto = null;
        if (await reader.ReadAsync())
        {
            dto = new EmployeeAvailabilityDTO
            {
                CompanyId = Convert.ToInt32(reader["CompanyId"]),
                UserId = Convert.ToInt32(reader["UserId"]),
                TimeZone = reader["TimeZone"] == DBNull.Value ? null : reader["TimeZone"]?.ToString(),
                MaxServicesPerDay = reader["MaxServicesPerDay"] == DBNull.Value ? null : Convert.ToInt32(reader["MaxServicesPerDay"]),
                Active = Convert.ToBoolean(reader["Active"]),
                Notes = reader["Notes"] == DBNull.Value ? null : reader["Notes"]?.ToString(),
            };
        }
        else
        {
            return null;
        }

        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                dto.Slots.Add(new EmployeeAvailabilitySlotDTO
                {
                    Id = Convert.ToInt64(reader["Id"]),
                    DayOfWeek = Convert.ToInt32(reader["DayOfWeek"]),
                    StartTime = TimeOnly.FromTimeSpan((TimeSpan)reader["StartTime"]),
                    EndTime = TimeOnly.FromTimeSpan((TimeSpan)reader["EndTime"]),
                    MaxServicesInSlot = reader["MaxServicesInSlot"] == DBNull.Value ? null : Convert.ToInt32(reader["MaxServicesInSlot"]),
                    Active = Convert.ToBoolean(reader["Active"])
                });
            }
        }

        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                dto.Exceptions.Add(new EmployeeAvailabilityExceptionDTO
                {
                    Id = Convert.ToInt64(reader["Id"]),
                    DateFrom = DateOnly.FromDateTime(Convert.ToDateTime(reader["DateFrom"])),
                    DateTo = DateOnly.FromDateTime(Convert.ToDateTime(reader["DateTo"])),
                    IsAvailable = Convert.ToBoolean(reader["IsAvailable"]),
                    Reason = reader["Reason"] == DBNull.Value ? null : reader["Reason"]?.ToString(),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }
        }

        return dto;
    }

    public async Task<bool> UpsertPaySchemeAsync(int companyId, string code, string name, string? unit, bool active)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@code", code, SqlDbType.NVarChar, 50),
            CreateParameter("@name", name, SqlDbType.NVarChar, 150),
            CreateParameter("@unit", (object?)unit ?? DBNull.Value, SqlDbType.NVarChar, 50),
            CreateParameter("@active", active, SqlDbType.Bit),
        };

        var success = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Hr.sp_upsert_pay_scheme,
            parameters,
            r => Convert.ToBoolean(r["Success"])
        );

        return success == true;
    }

    public async Task<List<PaySchemeDTO>> ListPaySchemesAsync(int companyId, bool onlyActive)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@only_active", onlyActive, SqlDbType.Bit),
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Hr.sp_list_pay_schemes,
            parameters,
            r => new PaySchemeDTO
            {
                Id = Convert.ToInt32(r["Id"]),
                CompanyId = Convert.ToInt32(r["CompanyId"]),
                Code = r["Code"]?.ToString() ?? string.Empty,
                Name = r["Name"]?.ToString() ?? string.Empty,
                Unit = r["Unit"] == DBNull.Value ? null : r["Unit"]?.ToString(),
                Active = Convert.ToBoolean(r["Active"]),
            }
        );
    }

    public async Task<long> CreateWorkLogAsync(int companyId, WorkLogDTO dto)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@user_id", dto.UserId, SqlDbType.Int),
            CreateParameter("@work_date", dto.WorkDate.Date, SqlDbType.Date),
            CreateParameter("@pay_scheme_id", dto.PaySchemeId, SqlDbType.Int),
            CreateParameter("@quantity", dto.Quantity, SqlDbType.Decimal),
            CreateParameter("@unit", (object?)dto.Unit ?? DBNull.Value, SqlDbType.NVarChar, 50),
            CreateParameter("@reference_type", (object?)dto.ReferenceType ?? DBNull.Value, SqlDbType.NVarChar, 50),
            CreateParameter("@reference_id", (object?)dto.ReferenceId ?? DBNull.Value, SqlDbType.NVarChar, 50),
            CreateParameter("@notes", (object?)dto.Notes ?? DBNull.Value, SqlDbType.NVarChar, 500),
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Hr.sp_create_work_log,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return insertedId;
    }

    public async Task<List<WorkLogDTO>> ListWorkLogsAsync(int companyId, int? userId, DateTime? fromDate, DateTime? toDate)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@user_id", (object?)userId ?? DBNull.Value, SqlDbType.Int),
            CreateParameter("@from_date", (object?)fromDate?.Date ?? DBNull.Value, SqlDbType.Date),
            CreateParameter("@to_date", (object?)toDate?.Date ?? DBNull.Value, SqlDbType.Date),
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Hr.sp_list_work_logs,
            parameters,
            r => new WorkLogDTO
            {
                Id = Convert.ToInt64(r["Id"]),
                CompanyId = Convert.ToInt32(r["CompanyId"]),
                UserId = Convert.ToInt32(r["UserId"]),
                WorkDate = Convert.ToDateTime(r["WorkDate"]),
                PaySchemeId = Convert.ToInt32(r["PaySchemeId"]),
                PaySchemeCode = r["PaySchemeCode"]?.ToString(),
                Quantity = Convert.ToDecimal(r["Quantity"]),
                Unit = r["Unit"] == DBNull.Value ? null : r["Unit"]?.ToString(),
                ReferenceType = r["ReferenceType"] == DBNull.Value ? null : r["ReferenceType"]?.ToString(),
                ReferenceId = r["ReferenceId"] == DBNull.Value ? null : r["ReferenceId"]?.ToString(),
                Notes = r["Notes"] == DBNull.Value ? null : r["Notes"]?.ToString(),
            }
        );
    }

    public async Task<long> CreateLoanAdvanceAsync(int companyId, LoanAdvanceDTO dto)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@user_id", dto.UserId, SqlDbType.Int),
            CreateParameter("@date", dto.Date.Date, SqlDbType.Date),
            CreateParameter("@amount", dto.Amount, SqlDbType.Decimal),
            CreateParameter("@notes", (object?)dto.Notes ?? DBNull.Value, SqlDbType.NVarChar, 500),
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Hr.sp_create_loan_advance,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return insertedId;
    }

    public async Task<List<LoanAdvanceDTO>> ListLoanAdvancesAsync(int companyId, int? userId, string? status)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@user_id", (object?)userId ?? DBNull.Value, SqlDbType.Int),
            CreateParameter("@status", (object?)status ?? DBNull.Value, SqlDbType.NVarChar, 20),
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Hr.sp_list_loan_advances,
            parameters,
            r => new LoanAdvanceDTO
            {
                Id = Convert.ToInt64(r["Id"]),
                CompanyId = Convert.ToInt32(r["CompanyId"]),
                UserId = Convert.ToInt32(r["UserId"]),
                Date = Convert.ToDateTime(r["Date"]),
                Amount = Convert.ToDecimal(r["Amount"]),
                Notes = r["Notes"] == DBNull.Value ? null : r["Notes"]?.ToString(),
                Status = r["Status"]?.ToString() ?? "open",
            }
        );
    }

    public async Task<long> CreateDeductionAsync(int companyId, DeductionDTO dto)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@user_id", dto.UserId, SqlDbType.Int),
            CreateParameter("@date", dto.Date.Date, SqlDbType.Date),
            CreateParameter("@amount", dto.Amount, SqlDbType.Decimal),
            CreateParameter("@type", (object?)dto.Type ?? DBNull.Value, SqlDbType.NVarChar, 50),
            CreateParameter("@notes", (object?)dto.Notes ?? DBNull.Value, SqlDbType.NVarChar, 500),
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Hr.sp_create_deduction,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return insertedId;
    }

    public async Task<List<DeductionDTO>> ListDeductionsAsync(int companyId, int? userId, string? type)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@user_id", (object?)userId ?? DBNull.Value, SqlDbType.Int),
            CreateParameter("@type", (object?)type ?? DBNull.Value, SqlDbType.NVarChar, 50),
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Hr.sp_list_deductions,
            parameters,
            r => new DeductionDTO
            {
                Id = Convert.ToInt64(r["Id"]),
                CompanyId = Convert.ToInt32(r["CompanyId"]),
                UserId = Convert.ToInt32(r["UserId"]),
                Date = Convert.ToDateTime(r["Date"]),
                Amount = Convert.ToDecimal(r["Amount"]),
                Type = r["Type"] == DBNull.Value ? null : r["Type"]?.ToString(),
                Notes = r["Notes"] == DBNull.Value ? null : r["Notes"]?.ToString(),
            }
        );
    }
}

