using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Pos.Cash;

public sealed record CloseCashSessionCommand(
    int CompanyId,
    long CashSessionId,
    decimal CountedClosingAmount,
    int? ClosedByUserId,
    string CashPaymentMethodCode = "CASH"
) : IRequest<Result<CashSessionCloseResultDTO>>;

