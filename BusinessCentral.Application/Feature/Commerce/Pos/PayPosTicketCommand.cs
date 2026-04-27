using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Pos;

public sealed record PayPosTicketCommand(
    int CompanyId,
    long TicketId,
    string Method,
    decimal Amount
) : IRequest<Result<bool>>;

