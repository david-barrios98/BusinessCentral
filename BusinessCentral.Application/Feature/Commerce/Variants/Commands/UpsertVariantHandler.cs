using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Variants.Commands;

public sealed class UpsertVariantHandler : IRequestHandler<UpsertVariantCommand, Result<long>>
{
    private readonly IProductVariantRepository _repo;

    public UpsertVariantHandler(IProductVariantRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<long>> Handle(UpsertVariantCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<long>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        if (request.ProductId <= 0)
            return Result<long>.Failure("ProductId inválido.", "VALIDATION", "Validation");

        if (string.IsNullOrWhiteSpace(request.Sku))
            return Result<long>.Failure("Sku requerido.", "VALIDATION", "Validation");

        var id = await _repo.UpsertAsync(
            request.CompanyId,
            request.Id,
            request.ProductId,
            request.Sku.Trim(),
            request.Barcode,
            request.VariantName,
            request.PriceOverride,
            request.CostOverride,
            request.Active
        );

        return Result<long>.Success(id);
    }
}

