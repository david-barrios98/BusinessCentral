using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Products;

public sealed class ListProductsHandler : IRequestHandler<ListProductsQuery, Result<PagedResult<ProductDTO>>>
{
    private readonly ICommerceRepository _commerce;

    public ListProductsHandler(ICommerceRepository commerce)
    {
        _commerce = commerce;
    }

    public async Task<Result<PagedResult<ProductDTO>>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 500 ? 50 : request.PageSize;

        var list = await _commerce.ListProductsAsync(request.CompanyId, request.OnlyActive, page, pageSize, request.Q);
        return Result<PagedResult<ProductDTO>>.Success(list);
    }
}

