using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Pos;

public sealed class GetPosTicketReceiptHandler : IRequestHandler<GetPosTicketReceiptQuery, Result<PosTicketReceiptDTO>>
{
    private readonly ICommerceRepository _commerce;

    public GetPosTicketReceiptHandler(ICommerceRepository commerce)
    {
        _commerce = commerce;
    }

    public async Task<Result<PosTicketReceiptDTO>> Handle(GetPosTicketReceiptQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0 || request.TicketId <= 0)
            return Result<PosTicketReceiptDTO>.Failure("CompanyId/TicketId inválido.", "VALIDATION", "Validation");

        var dto = await _commerce.GetPosTicketReceiptAsync(request.CompanyId, request.TicketId);
        if (dto is null)
            return Result<PosTicketReceiptDTO>.Failure("Ticket no encontrado.", "NOT_FOUND", "NotFound");

        return Result<PosTicketReceiptDTO>.Success(dto);
    }
}

