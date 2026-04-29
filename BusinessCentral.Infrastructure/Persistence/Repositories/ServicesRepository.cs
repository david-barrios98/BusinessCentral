using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.DTOs.Services;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Globalization;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class ServicesRepository : SqlConfigServer, IServicesRepository
{
    public ServicesRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<bool> UpsertServiceAsync(int companyId, string code, string name, decimal basePrice, bool active)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@code", code, SqlDbType.NVarChar, 50),
            CreateParameter("@name", name, SqlDbType.NVarChar, 150),
            CreateParameter("@base_price", basePrice, SqlDbType.Decimal),
            CreateParameter("@active", active, SqlDbType.Bit),
        };

        var success = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Services.sp_upsert_service,
            parameters,
            r => Convert.ToBoolean(r["Success"])
        );

        return success == true;
    }

    public async Task<List<ServiceDTO>> ListServicesAsync(int companyId, bool onlyActive)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@only_active", onlyActive, SqlDbType.Bit),
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Services.sp_list_services,
            parameters,
            r => new ServiceDTO
            {
                Id = Convert.ToInt32(r["Id"]),
                CompanyId = Convert.ToInt32(r["CompanyId"]),
                Code = r["Code"]?.ToString() ?? string.Empty,
                Name = r["Name"]?.ToString() ?? string.Empty,
                BasePrice = Convert.ToDecimal(r["BasePrice"]),
                Active = Convert.ToBoolean(r["Active"]),
            }
        );
    }

    public async Task<long> CreateServiceOrderAsync(int companyId, string? vehicleType, string? plate, string? customerName, string? fulfillmentMethodCode, string? fulfillmentDetails)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@vehicle_type", (object?)vehicleType ?? DBNull.Value, SqlDbType.NVarChar, 50),
            CreateParameter("@plate", (object?)plate ?? DBNull.Value, SqlDbType.NVarChar, 20),
            CreateParameter("@customer_name", (object?)customerName ?? DBNull.Value, SqlDbType.NVarChar, 150),
            CreateParameter("@fulfillment_method_code", (object?)fulfillmentMethodCode ?? DBNull.Value, SqlDbType.NVarChar, 30),
            CreateParameter("@fulfillment_details", (object?)fulfillmentDetails ?? DBNull.Value, SqlDbType.NVarChar, 500),
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Services.sp_create_service_order,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return insertedId;
    }

    public async Task<long> AddServiceOrderLineAsync(int companyId, long orderId, int serviceId, decimal quantity, decimal unitPrice, int? employeeUserId)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@order_id", orderId, SqlDbType.BigInt),
            CreateParameter("@service_id", serviceId, SqlDbType.Int),
            CreateParameter("@quantity", quantity, SqlDbType.Decimal),
            CreateParameter("@unit_price", unitPrice, SqlDbType.Decimal),
            CreateParameter("@employee_user_id", (object?)employeeUserId ?? DBNull.Value, SqlDbType.Int),
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Services.sp_add_service_order_line,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return insertedId;
    }

    public async Task<ServiceOrderDetailsDTO?> GetServiceOrderAsync(int companyId, long orderId)
    {
        using var connection = await OpenConnectionAsync();
        using var command = new SqlCommand(StoredProcedures.Services.sp_get_service_order, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddRange(new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@order_id", orderId, SqlDbType.BigInt),
        });

        using var reader = await command.ExecuteReaderAsync();

        ServiceOrderDTO? order = null;
        if (await reader.ReadAsync())
        {
            order = new ServiceOrderDTO
            {
                Id = Convert.ToInt64(reader["Id"]),
                CompanyId = Convert.ToInt32(reader["CompanyId"]),
                OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                VehicleType = reader["VehicleType"] == DBNull.Value ? null : reader["VehicleType"]?.ToString(),
                Plate = reader["Plate"] == DBNull.Value ? null : reader["Plate"]?.ToString(),
                CustomerName = reader["CustomerName"] == DBNull.Value ? null : reader["CustomerName"]?.ToString(),
                FulfillmentMethodCode = reader["FulfillmentMethodCode"] == DBNull.Value ? null : reader["FulfillmentMethodCode"]?.ToString(),
                FulfillmentDetails = reader["FulfillmentDetails"] == DBNull.Value ? null : reader["FulfillmentDetails"]?.ToString(),
                Status = reader["Status"]?.ToString() ?? "open",
                Total = Convert.ToDecimal(reader["Total"]),
                CreatedAt = reader["CreatedAt"] == DBNull.Value ? default : Convert.ToDateTime(reader["CreatedAt"]),
            };
        }
        else
        {
            return null;
        }

        var details = new ServiceOrderDetailsDTO { Order = order };

        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                details.Lines.Add(new ServiceOrderLineDTO
                {
                    Id = Convert.ToInt64(reader["Id"]),
                    OrderId = Convert.ToInt64(reader["OrderId"]),
                    ServiceId = Convert.ToInt32(reader["ServiceId"]),
                    ServiceName = reader["ServiceName"] == DBNull.Value ? null : reader["ServiceName"]?.ToString(),
                    Quantity = Convert.ToDecimal(reader["Quantity"]),
                    UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                    LineTotal = Convert.ToDecimal(reader["LineTotal"]),
                    EmployeeUserId = reader["EmployeeUserId"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["EmployeeUserId"]),
                });
            }
        }

        return details;
    }

    public async Task<PagedResult<ServiceOrderDTO>> ListServiceOrdersAsync(int companyId, string? status, int page, int pageSize)
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(StoredProcedures.Services.sp_list_service_orders, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(CreateParameter("@company_id", companyId, SqlDbType.Int));
        command.Parameters.Add(CreateParameter("@status", (object?)status ?? DBNull.Value, SqlDbType.NVarChar, 20));
        command.Parameters.Add(CreateParameter("@page", page, SqlDbType.Int));
        command.Parameters.Add(CreateParameter("@page_size", pageSize, SqlDbType.Int));

        var items = new List<ServiceOrderDTO>();
        long total = 0;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(new ServiceOrderDTO
            {
                Id = Convert.ToInt64(reader["Id"]),
                CompanyId = Convert.ToInt32(reader["CompanyId"]),
                OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                VehicleType = reader["VehicleType"] == DBNull.Value ? null : reader["VehicleType"]?.ToString(),
                Plate = reader["Plate"] == DBNull.Value ? null : reader["Plate"]?.ToString(),
                CustomerName = reader["CustomerName"] == DBNull.Value ? null : reader["CustomerName"]?.ToString(),
                FulfillmentMethodCode = reader["FulfillmentMethodCode"] == DBNull.Value ? null : reader["FulfillmentMethodCode"]?.ToString(),
                FulfillmentDetails = reader["FulfillmentDetails"] == DBNull.Value ? null : reader["FulfillmentDetails"]?.ToString(),
                Status = reader["Status"]?.ToString() ?? "open",
                Total = Convert.ToDecimal(reader["Total"]),
                CreatedAt = reader["CreatedAt"] == DBNull.Value ? default : Convert.ToDateTime(reader["CreatedAt"]),
            });
        }

        if (await reader.NextResultAsync() && await reader.ReadAsync())
            total = Convert.ToInt64(reader["Total"]);

        return new PagedResult<ServiceOrderDTO>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }

    public async Task<ServiceCompanySettingsDTO> GetCompanySettingsAsync(int companyId)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
        };

        var dto = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Services.sp_get_service_company_settings,
            parameters,
            r => new ServiceCompanySettingsDTO
            {
                CompanyId = Convert.ToInt32(r["CompanyId"]),
                EnableAgendas = Convert.ToBoolean(r["EnableAgendas"]),
                EnableCoverage = Convert.ToBoolean(r["EnableCoverage"]),
                EnableShifts = Convert.ToBoolean(r["EnableShifts"]),
                ShiftFrequencyType = r["ShiftFrequencyType"]?.ToString() ?? "WEEKLY",
                ShiftSlotMinutes = Convert.ToInt32(r["ShiftSlotMinutes"]),
                CreatedAt = Convert.ToDateTime(r["CreatedAt"]),
                UpdatedAt = Convert.ToDateTime(r["UpdatedAt"])
            });

        return dto ?? new ServiceCompanySettingsDTO { CompanyId = companyId };
    }

    public async Task<bool> UpdateCompanySettingsAsync(int companyId, UpdateServiceCompanySettingsRequest request)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@enable_agendas", request.EnableAgendas, SqlDbType.Bit),
            CreateParameter("@enable_coverage", request.EnableCoverage, SqlDbType.Bit),
            CreateParameter("@enable_shifts", request.EnableShifts, SqlDbType.Bit),
            CreateParameter("@shift_frequency_type", request.ShiftFrequencyType, SqlDbType.NVarChar, 20),
            CreateParameter("@shift_slot_minutes", request.ShiftSlotMinutes, SqlDbType.Int),
        };

        var ok = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Services.sp_upsert_service_company_settings,
            parameters,
            r => Convert.ToBoolean(r["Success"]));

        return ok == true;
    }

    public async Task<List<ServiceCoverageAreaDTO>> ListCoverageAreasAsync(int companyId, bool onlyActive = true)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@only_active", onlyActive, SqlDbType.Bit),
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Services.sp_list_service_coverage_areas,
            parameters,
            r => new ServiceCoverageAreaDTO
            {
                Id = Convert.ToInt32(r["Id"]),
                CompanyId = Convert.ToInt32(r["CompanyId"]),
                CoverageType = r["CoverageType"]?.ToString() ?? "CITY",
                CountryId = r["CountryId"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["CountryId"]),
                DepartmentId = r["DepartmentId"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["DepartmentId"]),
                CityId = r["CityId"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["CityId"]),
                Notes = r["Notes"] == DBNull.Value ? null : r["Notes"]?.ToString(),
                Active = Convert.ToBoolean(r["Active"]),
                CreatedAt = Convert.ToDateTime(r["CreatedAt"]),
                UpdatedAt = Convert.ToDateTime(r["UpdatedAt"]),
            });
    }

    public async Task<int> UpsertCoverageAreaAsync(int companyId, UpsertServiceCoverageAreaRequest request)
    {
        object idParam = request.Id is > 0 ? request.Id.Value : DBNull.Value;

        var parameters = new[]
        {
            CreateParameter("@Id", idParam, SqlDbType.Int),
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@coverage_type", request.CoverageType, SqlDbType.NVarChar, 20),
            CreateParameter("@country_id", (object?)request.CountryId ?? DBNull.Value, SqlDbType.Int),
            CreateParameter("@department_id", (object?)request.DepartmentId ?? DBNull.Value, SqlDbType.Int),
            CreateParameter("@city_id", (object?)request.CityId ?? DBNull.Value, SqlDbType.Int),
            CreateParameter("@notes", (object?)request.Notes ?? DBNull.Value, SqlDbType.NVarChar, 300),
            CreateParameter("@active", request.Active, SqlDbType.Bit),
        };

        int? newId = await ExecuteStoredProcedureSingleAsync<int>(
            StoredProcedures.Services.sp_upsert_service_coverage_area,
            parameters,
            r => Convert.ToInt32(r["Id"]));

        return newId ?? 0;
    }

    public async Task DeleteCoverageAreaAsync(int companyId, int id)
    {
        var parameters = new[]
        {
            CreateParameter("@Id", id, SqlDbType.Int),
            CreateParameter("@company_id", companyId, SqlDbType.Int),
        };

        await ExecuteStoredProcedureNonQueryAsync(
            StoredProcedures.Services.sp_delete_service_coverage_area,
            parameters);
    }

    public async Task<List<ServiceShiftTemplateDTO>> ListShiftTemplatesAsync(int companyId, bool onlyActive = true)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@only_active", onlyActive, SqlDbType.Bit),
        };

        return await ExecuteStoredProcedureAsync(
            StoredProcedures.Services.sp_list_service_shift_templates,
            parameters,
            r => new ServiceShiftTemplateDTO
            {
                Id = Convert.ToInt32(r["Id"]),
                CompanyId = Convert.ToInt32(r["CompanyId"]),
                Code = r["Code"]?.ToString() ?? string.Empty,
                Name = r["Name"]?.ToString() ?? string.Empty,
                StartTime = ((TimeSpan)r["StartTime"]).ToString("c", CultureInfo.InvariantCulture),
                EndTime = ((TimeSpan)r["EndTime"]).ToString("c", CultureInfo.InvariantCulture),
                FrequencyType = r["FrequencyType"]?.ToString() ?? "WEEKLY",
                Interval = Convert.ToInt32(r["Interval"]),
                DaysOfWeekMask = Convert.ToInt32(r["DaysOfWeekMask"]),
                Active = Convert.ToBoolean(r["Active"]),
                CreatedAt = Convert.ToDateTime(r["CreatedAt"]),
                UpdatedAt = Convert.ToDateTime(r["UpdatedAt"]),
            });
    }

    public async Task<int> UpsertShiftTemplateAsync(int companyId, UpsertServiceShiftTemplateRequest request)
    {
        object idParam = request.Id is > 0 ? request.Id.Value : DBNull.Value;
        var start = TimeSpan.Parse(request.StartTime, CultureInfo.InvariantCulture);
        var end = TimeSpan.Parse(request.EndTime, CultureInfo.InvariantCulture);

        var parameters = new[]
        {
            CreateParameter("@Id", idParam, SqlDbType.Int),
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@code", request.Code, SqlDbType.NVarChar, 50),
            CreateParameter("@name", request.Name, SqlDbType.NVarChar, 150),
            CreateParameter("@start_time", start, SqlDbType.Time),
            CreateParameter("@end_time", end, SqlDbType.Time),
            CreateParameter("@frequency_type", request.FrequencyType, SqlDbType.NVarChar, 20),
            CreateParameter("@interval", request.Interval, SqlDbType.Int),
            CreateParameter("@days_of_week_mask", request.DaysOfWeekMask, SqlDbType.Int),
            CreateParameter("@active", request.Active, SqlDbType.Bit),
        };

        int? newId = await ExecuteStoredProcedureSingleAsync<int>(
            StoredProcedures.Services.sp_upsert_service_shift_template,
            parameters,
            r => Convert.ToInt32(r["Id"]));

        return newId ?? 0;
    }

    public async Task DeleteShiftTemplateAsync(int companyId, int id)
    {
        var parameters = new[]
        {
            CreateParameter("@Id", id, SqlDbType.Int),
            CreateParameter("@company_id", companyId, SqlDbType.Int),
        };

        await ExecuteStoredProcedureNonQueryAsync(
            StoredProcedures.Services.sp_delete_service_shift_template,
            parameters);
    }
}

