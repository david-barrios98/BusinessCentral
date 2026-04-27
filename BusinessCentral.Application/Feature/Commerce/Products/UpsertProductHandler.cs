using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Products;

public sealed class UpsertProductHandler : IRequestHandler<UpsertProductCommand, Result<bool>>
{
    private readonly ICommerceRepository _commerce;

    public UpsertProductHandler(ICommerceRepository commerce)
    {
        _commerce = commerce;
    }

    public async Task<Result<bool>> Handle(UpsertProductCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Sku) || string.IsNullOrWhiteSpace(request.Name))
            return Result<bool>.Failure("Sku y Name son requeridos.", "COM_PRODUCT_VALIDATION", "Validation");

        var ok = await _commerce.UpsertProductAsync(request.CompanyId, request.Sku, request.Name, request.Unit, request.Price, request.Active);
        return ok
            ? Result<bool>.Success(true)
            : Result<bool>.Failure("No se pudo guardar el producto.", "COM_PRODUCT_UPSERT_FAILED", "Conflict");
    }
}

