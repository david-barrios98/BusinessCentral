using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Variants.Queries;

public sealed record ListVariantsQuery(int CompanyId, int? ProductId = null, bool OnlyActive = true, string? Q = null)
    : IRequest<Result<List<ProductVariantListItemDTO>>>;

