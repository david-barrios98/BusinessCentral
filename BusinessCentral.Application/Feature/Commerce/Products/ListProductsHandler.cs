using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Products;

public sealed class ListProductsHandler : IRequestHandler<ListProductsQuery, Result<List<ProductDTO>>>
{
    private readonly ICommerceRepository _commerce;

    public ListProductsHandler(ICommerceRepository commerce)
    {
        _commerce = commerce;
    }

    public async Task<Result<List<ProductDTO>>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var list = await _commerce.ListProductsAsync(request.CompanyId, request.OnlyActive);
        return Result<List<ProductDTO>>.Success(list);
    }
}

