using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Purchasing.Commands;

public sealed class PostPurchaseReceiptHandler : IRequestHandler<PostPurchaseReceiptCommand, Result<bool>>
{
    private readonly IPurchaseReceivingRepository _repo;

    public PostPurchaseReceiptHandler(IPurchaseReceivingRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<bool>> Handle(PostPurchaseReceiptCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0 || request.ReceiptId <= 0)
            return Result<bool>.Failure("CompanyId/ReceiptId inválido.", "VALIDATION", "Validation");

        var ok = await _repo.PostReceiptAsync(request.CompanyId, request.ReceiptId);
        return Result<bool>.Success(ok);
    }
}

