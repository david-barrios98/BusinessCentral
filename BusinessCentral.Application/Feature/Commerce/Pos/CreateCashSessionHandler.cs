using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Pos;

public sealed class CreateCashSessionHandler : IRequestHandler<CreateCashSessionCommand, Result<long>>
{
    private readonly ICommerceRepository _commerce;

    public CreateCashSessionHandler(ICommerceRepository commerce)
    {
        _commerce = commerce;
    }

    public async Task<Result<long>> Handle(CreateCashSessionCommand request, CancellationToken cancellationToken)
    {
        var id = await _commerce.CreateCashSessionAsync(request.CompanyId, request.OpenedByUserId, request.OpeningAmount);
        return id > 0
            ? Result<long>.Success(id)
            : Result<long>.Failure("No se pudo abrir caja.", "POS_CASH_OPEN_FAILED", "Conflict");
    }
}

