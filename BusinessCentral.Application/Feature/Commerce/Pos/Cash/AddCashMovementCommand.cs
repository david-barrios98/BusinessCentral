using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Pos.Cash;

public sealed record AddCashMovementCommand(
    int CompanyId,
    long CashSessionId,
    string Direction,
    string? ReasonCode,
    decimal Amount,
    string? ReferenceType,
    string? ReferenceId,
    string? Notes,
    int? PerformedByUserId
) : IRequest<Result<long>>;

