using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.DTOs.Common;

namespace BusinessCentral.Application.Ports.Outbound;

public interface ICommerceRepository
{
    Task<bool> UpsertProductAsync(int companyId, string sku, string name, string? unit, decimal price, bool active, int? performedByUserId);
    Task<PagedResult<ProductDTO>> ListProductsAsync(int companyId, bool onlyActive, int page, int pageSize, string? q = null);

    Task<long> CreateCashSessionAsync(int companyId, int? openedByUserId, decimal openingAmount);
    Task<long> CreatePosTicketAsync(int companyId, long? cashSessionId, string? fulfillmentMethodCode, string? fulfillmentDetails);
    Task<long> AddPosTicketLineAsync(int companyId, long ticketId, int productId, decimal quantity, decimal unitPrice);
    Task<bool> PayPosTicketAsync(int companyId, long ticketId, string method, decimal amount);
    Task<PosTicketReceiptDTO?> GetPosTicketReceiptAsync(int companyId, long ticketId);
}

