using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Variants.Queries;

public sealed record ListVariantsQuery(int CompanyId, int? ProductId = null, bool OnlyActive = true, int Page = 1, int PageSize = 50, string? Q = null)
    : IRequest<Result<PagedResult<ProductVariantListItemDTO>>>;

