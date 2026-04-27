using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Products;

public sealed record ListProductsQuery(int CompanyId, bool OnlyActive, int Page = 1, int PageSize = 50, string? Q = null)
    : IRequest<Result<PagedResult<ProductDTO>>>;

