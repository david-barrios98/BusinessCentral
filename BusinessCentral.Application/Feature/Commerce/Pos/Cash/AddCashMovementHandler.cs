using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Pos.Cash;

public sealed class AddCashMovementHandler : IRequestHandler<AddCashMovementCommand, Result<long>>
{
    private readonly ICommerceRepository _commerce;

    public AddCashMovementHandler(ICommerceRepository commerce)
    {
        _commerce = commerce;
    }

    public async Task<Result<long>> Handle(AddCashMovementCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0 || request.CashSessionId <= 0)
            return Result<long>.Failure("CompanyId/CashSessionId inválido.", "VALIDATION", "Validation");

        if (string.IsNullOrWhiteSpace(request.Direction))
            return Result<long>.Failure("Direction requerido.", "VALIDATION", "Validation");

        if (request.Amount <= 0)
            return Result<long>.Failure("Amount debe ser > 0.", "VALIDATION", "Validation");

        var direction = request.Direction.Trim().ToUpperInvariant();
        if (direction is not ("IN" or "OUT"))
            return Result<long>.Failure("Direction debe ser IN u OUT.", "VALIDATION", "Validation");

        var id = await _commerce.AddCashMovementAsync(
            request.CompanyId,
            request.CashSessionId,
            direction,
            string.IsNullOrWhiteSpace(request.ReasonCode) ? null : request.ReasonCode.Trim().ToUpperInvariant(),
            request.Amount,
            request.ReferenceType,
            request.ReferenceId,
            request.Notes,
            request.PerformedByUserId);

        return id > 0
            ? Result<long>.Success(id)
            : Result<long>.Failure("No se pudo registrar el movimiento.", "CASH_MOVEMENT_FAILED", "Conflict");
    }
}

