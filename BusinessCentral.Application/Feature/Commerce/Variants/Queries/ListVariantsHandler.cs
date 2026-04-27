using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Variants.Queries;

public sealed class ListVariantsHandler : IRequestHandler<ListVariantsQuery, Result<PagedResult<ProductVariantListItemDTO>>>
{
    private readonly IProductVariantRepository _repo;

    public ListVariantsHandler(IProductVariantRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<PagedResult<ProductVariantListItemDTO>>> Handle(ListVariantsQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<PagedResult<ProductVariantListItemDTO>>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 500 ? 50 : request.PageSize;

        var data = await _repo.ListAsync(request.CompanyId, request.ProductId, request.OnlyActive, page, pageSize, request.Q);
        return Result<PagedResult<ProductVariantListItemDTO>>.Success(data);
    }
}

