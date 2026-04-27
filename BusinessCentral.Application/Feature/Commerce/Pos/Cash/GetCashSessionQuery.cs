using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Pos.Cash;

public sealed record GetCashSessionQuery(int CompanyId, long CashSessionId)
    : IRequest<Result<CashSessionDetailsDTO>>;

