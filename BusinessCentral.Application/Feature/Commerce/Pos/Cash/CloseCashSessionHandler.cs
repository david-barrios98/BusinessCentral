using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Pos.Cash;

public sealed class CloseCashSessionHandler : IRequestHandler<CloseCashSessionCommand, Result<CashSessionCloseResultDTO>>
{
    private readonly ICommerceRepository _commerce;

    public CloseCashSessionHandler(ICommerceRepository commerce)
    {
        _commerce = commerce;
    }

    public async Task<Result<CashSessionCloseResultDTO>> Handle(CloseCashSessionCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0 || request.CashSessionId <= 0)
            return Result<CashSessionCloseResultDTO>.Failure("CompanyId/CashSessionId inválido.", "VALIDATION", "Validation");

        if (request.CountedClosingAmount < 0)
            return Result<CashSessionCloseResultDTO>.Failure("CountedClosingAmount inválido.", "VALIDATION", "Validation");

        var code = string.IsNullOrWhiteSpace(request.CashPaymentMethodCode) ? "CASH" : request.CashPaymentMethodCode.Trim().ToUpperInvariant();

        var dto = await _commerce.CloseCashSessionAsync(
            request.CompanyId,
            request.CashSessionId,
            request.CountedClosingAmount,
            request.ClosedByUserId,
            code);

        if (dto is null || !dto.Success)
            return Result<CashSessionCloseResultDTO>.Failure("No se pudo cerrar la caja.", "CASH_CLOSE_FAILED", "Conflict");

        return Result<CashSessionCloseResultDTO>.Success(dto);
    }
}

