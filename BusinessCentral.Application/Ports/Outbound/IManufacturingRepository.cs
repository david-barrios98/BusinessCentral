using BusinessCentral.Application.DTOs.Manufacturing;

namespace BusinessCentral.Application.Ports.Outbound;

public interface IManufacturingRepository
{
    Task<long> UpsertRecipeAsync(int companyId, long? id, int outputProductId, long? outputVariantId, decimal outputQuantity, string? notes, bool active);
    Task<bool> SetRecipeItemsAsync(int companyId, long recipeId, List<RecipeItemUpsertDTO> items);
    Task<long> CreateBatchAsync(int companyId, long? recipeId, int outputProductId, long? outputVariantId, decimal quantityProduced, long? toLocationId, DateTime batchDate, string? notes);
    Task<bool> PostBatchAsync(int companyId, long batchId);
    Task<RecipeCostReportDTO> GetRecipeCostAsync(int companyId, long recipeId, decimal quantity);
}

