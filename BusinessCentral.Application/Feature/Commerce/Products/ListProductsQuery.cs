using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Products;

public sealed record ListProductsQuery(int CompanyId, bool OnlyActive) : IRequest<Result<List<ProductDTO>>>;

