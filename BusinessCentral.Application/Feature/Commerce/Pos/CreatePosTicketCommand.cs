using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Pos;

public sealed record CreatePosTicketCommand(int CompanyId, long? CashSessionId) : IRequest<Result<long>>;

