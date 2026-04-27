using BusinessCentral.Application.DTOs.Services;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

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
}

