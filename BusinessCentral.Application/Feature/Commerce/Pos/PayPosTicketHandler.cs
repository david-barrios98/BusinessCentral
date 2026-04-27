using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Pos;

public sealed class PayPosTicketHandler : IRequestHandler<PayPosTicketCommand, Result<bool>>
{
    private readonly ICommerceRepository _commerce;

    public PayPosTicketHandler(ICommerceRepository commerce)
    {
        _commerce = commerce;
    }

    public async Task<Result<bool>> Handle(PayPosTicketCommand request, CancellationToken cancellationToken)
    {
        if (request.TicketId <= 0 || string.IsNullOrWhiteSpace(request.Method) || request.Amount <= 0)
            return Result<bool>.Failure("Datos inválidos para el pago.", "POS_PAY_VALIDATION", "Validation");

        var ok = await _commerce.PayPosTicketAsync(request.CompanyId, request.TicketId, request.Method, request.Amount);
        return ok
            ? Result<bool>.Success(true)
            : Result<bool>.Failure("No se pudo pagar el ticket.", "POS_PAY_FAILED", "Conflict");
    }
}

