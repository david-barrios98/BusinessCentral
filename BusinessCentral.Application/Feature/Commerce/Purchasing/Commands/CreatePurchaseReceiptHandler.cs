using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Purchasing.Commands;

public sealed class CreatePurchaseReceiptHandler : IRequestHandler<CreatePurchaseReceiptCommand, Result<long>>
{
    private readonly IPurchaseReceivingRepository _repo;

    public CreatePurchaseReceiptHandler(IPurchaseReceivingRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<long>> Handle(CreatePurchaseReceiptCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<long>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        var id = await _repo.CreateReceiptAsync(
            request.CompanyId,
            request.SupplierId,
            request.ReceiptDate,
            request.SupplierInvoiceNumber,
            request.DefaultToLocationId,
            request.CreatedByUserId
        );

        return Result<long>.Success(id);
    }
}

