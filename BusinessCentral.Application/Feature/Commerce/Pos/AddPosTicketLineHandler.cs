using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Pos;

public sealed class AddPosTicketLineHandler : IRequestHandler<AddPosTicketLineCommand, Result<long>>
{
    private readonly ICommerceRepository _commerce;

    public AddPosTicketLineHandler(ICommerceRepository commerce)
    {
        _commerce = commerce;
    }

    public async Task<Result<long>> Handle(AddPosTicketLineCommand request, CancellationToken cancellationToken)
    {
        if (request.TicketId <= 0 || request.ProductId <= 0 || request.Quantity <= 0 || request.UnitPrice < 0)
            return Result<long>.Failure("Datos inválidos para la línea.", "POS_TICKETLINE_VALIDATION", "Validation");

        var id = await _commerce.AddPosTicketLineAsync(
            request.CompanyId,
            request.TicketId,
            request.ProductId,
            request.Quantity,
            request.UnitPrice
        );

        return id > 0
            ? Result<long>.Success(id)
            : Result<long>.Failure("No se pudo agregar la línea.", "POS_TICKETLINE_CREATE_FAILED", "Conflict");
    }
}

