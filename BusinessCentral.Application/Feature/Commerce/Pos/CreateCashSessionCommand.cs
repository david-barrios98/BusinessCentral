using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Pos;

public sealed record CreateCashSessionCommand(
    int CompanyId,
    int? OpenedByUserId,
    decimal OpeningAmount
) : IRequest<Result<long>>;

