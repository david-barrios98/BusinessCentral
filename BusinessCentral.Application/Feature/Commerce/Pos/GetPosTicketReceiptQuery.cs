using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Pos;

public sealed record GetPosTicketReceiptQuery(int CompanyId, long TicketId)
    : IRequest<Result<PosTicketReceiptDTO>>;

