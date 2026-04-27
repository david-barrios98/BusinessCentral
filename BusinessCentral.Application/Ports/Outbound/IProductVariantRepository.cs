using BusinessCentral.Application.DTOs.Commerce;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IProductVariantRepository
{
    Task<long> UpsertAsync(int companyId, long? id, int productId, string sku, string? barcode, string? variantName, decimal? priceOverride, decimal? costOverride, bool active);
    Task<List<ProductVariantListItemDTO>> ListAsync(int companyId, int? productId = null, bool onlyActive = true, string? q = null);
}

