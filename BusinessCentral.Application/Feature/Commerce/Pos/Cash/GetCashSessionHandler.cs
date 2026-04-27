using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Pos.Cash;

public sealed class GetCashSessionHandler : IRequestHandler<GetCashSessionQuery, Result<CashSessionDetailsDTO>>
{
    private readonly ICommerceRepository _commerce;

    public GetCashSessionHandler(ICommerceRepository commerce)
    {
        _commerce = commerce;
    }

    public async Task<Result<CashSessionDetailsDTO>> Handle(GetCashSessionQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0 || request.CashSessionId <= 0)
            return Result<CashSessionDetailsDTO>.Failure("CompanyId/CashSessionId inválido.", "VALIDATION", "Validation");

        var dto = await _commerce.GetCashSessionAsync(request.CompanyId, request.CashSessionId);
        if (dto is null)
            return Result<CashSessionDetailsDTO>.Failure("Caja no encontrada.", "NOT_FOUND", "NotFound");

        return Result<CashSessionDetailsDTO>.Success(dto);
    }
}

