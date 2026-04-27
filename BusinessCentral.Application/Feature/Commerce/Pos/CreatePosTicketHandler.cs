using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Pos;

public sealed class CreatePosTicketHandler : IRequestHandler<CreatePosTicketCommand, Result<long>>
{
    private readonly ICommerceRepository _commerce;

    public CreatePosTicketHandler(ICommerceRepository commerce)
    {
        _commerce = commerce;
    }

    public async Task<Result<long>> Handle(CreatePosTicketCommand request, CancellationToken cancellationToken)
    {
        var method = string.IsNullOrWhiteSpace(request.FulfillmentMethodCode) ? null : request.FulfillmentMethodCode.Trim().ToUpperInvariant();
        var details = string.IsNullOrWhiteSpace(request.FulfillmentDetails) ? null : request.FulfillmentDetails.Trim();

        var id = await _commerce.CreatePosTicketAsync(request.CompanyId, request.CashSessionId, method, details);
        return id > 0
            ? Result<long>.Success(id)
            : Result<long>.Failure("No se pudo crear el ticket.", "POS_TICKET_CREATE_FAILED", "Conflict");
    }
}

