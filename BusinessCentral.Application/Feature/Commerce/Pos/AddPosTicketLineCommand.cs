using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Pos;

public sealed record AddPosTicketLineCommand(
    int CompanyId,
    long TicketId,
    int ProductId,
    decimal Quantity,
    decimal UnitPrice
) : IRequest<Result<long>>;

