using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

namespace BusinessCentral.Infrastructure.Persistence.Repositories;

public sealed class CommerceRepository : SqlConfigServer, ICommerceRepository
{
    public CommerceRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<bool> UpsertProductAsync(int companyId, string sku, string name, string? unit, decimal price, bool active, int? performedByUserId)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@sku", sku, SqlDbType.NVarChar, 50),
            CreateParameter("@name", name, SqlDbType.NVarChar, 200),
            CreateParameter("@unit", (object?)unit ?? DBNull.Value, SqlDbType.NVarChar, 20),
            CreateParameter("@price", price, SqlDbType.Decimal),
            CreateParameter("@active", active, SqlDbType.Bit),
            CreateParameter("@performed_by_user_id", (object?)performedByUserId ?? DBNull.Value, SqlDbType.Int),
        };

        var success = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Commerce.sp_upsert_product,
            parameters,
            r => Convert.ToBoolean(r["Success"])
        );

        return success == true;
    }

    public async Task<PagedResult<ProductDTO>> ListProductsAsync(int companyId, bool onlyActive, int page, int pageSize, string? q = null)
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(StoredProcedures.Commerce.sp_list_products, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(CreateParameter("@company_id", companyId, SqlDbType.Int));
        command.Parameters.Add(CreateParameter("@only_active", onlyActive, SqlDbType.Bit));
        command.Parameters.Add(CreateParameter("@page", page, SqlDbType.Int));
        command.Parameters.Add(CreateParameter("@page_size", pageSize, SqlDbType.Int));
        command.Parameters.Add(CreateParameter("@q", (object?)q ?? DBNull.Value, SqlDbType.NVarChar, 100));

        var items = new List<ProductDTO>();
        long total = 0;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(new ProductDTO
            {
                Id = Convert.ToInt32(reader["Id"]),
                CompanyId = Convert.ToInt32(reader["CompanyId"]),
                Sku = reader["Sku"]?.ToString() ?? string.Empty,
                Name = reader["Name"]?.ToString() ?? string.Empty,
                Unit = reader["Unit"] == DBNull.Value ? null : reader["Unit"]?.ToString(),
                Price = Convert.ToDecimal(reader["Price"]),
                Active = Convert.ToBoolean(reader["Active"]),
            });
        }

        if (await reader.NextResultAsync() && await reader.ReadAsync())
            total = Convert.ToInt64(reader["Total"]);

        return new PagedResult<ProductDTO>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }

    public async Task<long> CreateCashSessionAsync(int companyId, int? openedByUserId, decimal openingAmount)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@opened_by_user_id", (object?)openedByUserId ?? DBNull.Value, SqlDbType.Int),
            CreateParameter("@opening_amount", openingAmount, SqlDbType.Decimal),
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Commerce.sp_create_cash_session,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return insertedId;
    }

    public async Task<PosTicketReceiptDTO?> GetPosTicketReceiptAsync(int companyId, long ticketId)
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(StoredProcedures.Commerce.sp_get_pos_ticket, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(CreateParameter("@company_id", companyId, SqlDbType.Int));
        command.Parameters.Add(CreateParameter("@ticket_id", ticketId, SqlDbType.BigInt));

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        var receipt = new PosTicketReceiptDTO
        {
            Ticket = new PosTicketHeaderDTO
            {
                Id = Convert.ToInt64(reader["Id"]),
                CompanyId = Convert.ToInt32(reader["CompanyId"]),
                CashSessionId = reader["CashSessionId"] == DBNull.Value ? null : Convert.ToInt64(reader["CashSessionId"]),
                TicketDate = Convert.ToDateTime(reader["TicketDate"]),
                Status = reader["Status"]?.ToString() ?? "open",
                Total = Convert.ToDecimal(reader["Total"]),
                FulfillmentMethodCode = reader["FulfillmentMethodCode"] == DBNull.Value ? null : reader["FulfillmentMethodCode"]?.ToString(),
                FulfillmentDetails = reader["FulfillmentDetails"] == DBNull.Value ? null : reader["FulfillmentDetails"]?.ToString(),
            }
        };

        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                receipt.Lines.Add(new PosTicketReceiptLineDTO
                {
                    Id = Convert.ToInt64(reader["Id"]),
                    TicketId = Convert.ToInt64(reader["TicketId"]),
                    ProductId = Convert.ToInt32(reader["ProductId"]),
                    ProductSku = reader["ProductSku"]?.ToString() ?? string.Empty,
                    ProductName = reader["ProductName"]?.ToString() ?? string.Empty,
                    Quantity = Convert.ToDecimal(reader["Quantity"]),
                    UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                    LineTotal = Convert.ToDecimal(reader["LineTotal"]),
                });
            }
        }

        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                receipt.Payments.Add(new PosTicketPaymentDTO
                {
                    Id = Convert.ToInt64(reader["Id"]),
                    TicketId = Convert.ToInt64(reader["TicketId"]),
                    Method = reader["Method"]?.ToString() ?? string.Empty,
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    PaidAt = Convert.ToDateTime(reader["PaidAt"]),
                });
            }
        }

        return receipt;
    }

    public async Task<long> CreatePosTicketAsync(int companyId, long? cashSessionId, string? fulfillmentMethodCode, string? fulfillmentDetails)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@cash_session_id", (object?)cashSessionId ?? DBNull.Value, SqlDbType.BigInt),
            CreateParameter("@fulfillment_method_code", (object?)fulfillmentMethodCode ?? DBNull.Value, SqlDbType.NVarChar, 30),
            CreateParameter("@fulfillment_details", (object?)fulfillmentDetails ?? DBNull.Value, SqlDbType.NVarChar, 500),
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Commerce.sp_create_pos_ticket,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return insertedId;
    }

    public async Task<long> AddPosTicketLineAsync(int companyId, long ticketId, int productId, decimal quantity, decimal unitPrice)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@ticket_id", ticketId, SqlDbType.BigInt),
            CreateParameter("@product_id", productId, SqlDbType.Int),
            CreateParameter("@quantity", quantity, SqlDbType.Decimal),
            CreateParameter("@unit_price", unitPrice, SqlDbType.Decimal),
        };

        var insertedId = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Commerce.sp_add_pos_ticket_line,
            parameters,
            r => Convert.ToInt64(r["InsertedId"])
        );

        return insertedId;
    }

    public async Task<bool> PayPosTicketAsync(int companyId, long ticketId, string method, decimal amount)
    {
        var parameters = new[]
        {
            CreateParameter("@company_id", companyId, SqlDbType.Int),
            CreateParameter("@ticket_id", ticketId, SqlDbType.BigInt),
            CreateParameter("@method", method, SqlDbType.NVarChar, 20),
            CreateParameter("@amount", amount, SqlDbType.Decimal),
        };

        var success = await ExecuteStoredProcedureSingleAsync(
            StoredProcedures.Commerce.sp_pay_pos_ticket,
            parameters,
            r => Convert.ToBoolean(r["Success"])
        );

        return success == true;
    }
}

