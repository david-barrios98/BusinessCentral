using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Purchasing.Commands;

public sealed class AddPurchaseReceiptLineHandler : IRequestHandler<AddPurchaseReceiptLineCommand, Result<long>>
{
    private readonly IPurchaseReceivingRepository _repo;

    public AddPurchaseReceiptLineHandler(IPurchaseReceivingRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<long>> Handle(AddPurchaseReceiptLineCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<long>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        if (request.ReceiptId <= 0 || request.ProductId <= 0)
            return Result<long>.Failure("ReceiptId/ProductId inválido.", "VALIDATION", "Validation");

        if (request.Quantity <= 0 || request.UnitCost < 0)
            return Result<long>.Failure("Cantidad/costo inválidos.", "VALIDATION", "Validation");

        var id = await _repo.AddReceiptLineAsync(
            request.CompanyId,
            request.ReceiptId,
            request.ProductId,
            request.VariantId,
            request.Quantity,
            request.UnitCost,
            request.ToLocationId,
            request.Notes
        );

        return Result<long>.Success(id);
    }
}

