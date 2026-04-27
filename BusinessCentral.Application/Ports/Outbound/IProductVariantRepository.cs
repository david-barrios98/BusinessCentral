using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.DTOs.Common;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IProductVariantRepository
{
    Task<long> UpsertAsync(int companyId, long? id, int productId, string sku, string? barcode, string? variantName, decimal? priceOverride, decimal? costOverride, bool active);
    Task<PagedResult<ProductVariantListItemDTO>> ListAsync(int companyId, int? productId = null, bool onlyActive = true, int page = 1, int pageSize = 50, string? q = null);
}

