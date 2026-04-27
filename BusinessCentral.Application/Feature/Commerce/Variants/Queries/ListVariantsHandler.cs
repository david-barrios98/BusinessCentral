using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Variants.Queries;

public sealed class ListVariantsHandler : IRequestHandler<ListVariantsQuery, Result<List<ProductVariantListItemDTO>>>
{
    private readonly IProductVariantRepository _repo;

    public ListVariantsHandler(IProductVariantRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<List<ProductVariantListItemDTO>>> Handle(ListVariantsQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<List<ProductVariantListItemDTO>>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        var data = await _repo.ListAsync(request.CompanyId, request.ProductId, request.OnlyActive, request.Q);
        return Result<List<ProductVariantListItemDTO>>.Success(data);
    }
}

