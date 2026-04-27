using BusinessCentral.Application.DTOs.Services;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Services.Orders;

public sealed class GetServiceOrderHandler : IRequestHandler<GetServiceOrderQuery, Result<ServiceOrderDetailsDTO>>
{
    private readonly IServicesRepository _services;

    public GetServiceOrderHandler(IServicesRepository services)
    {
        _services = services;
    }

    public async Task<Result<ServiceOrderDetailsDTO>> Handle(GetServiceOrderQuery request, CancellationToken cancellationToken)
    {
        var details = await _services.GetServiceOrderAsync(request.CompanyId, request.OrderId);
        return details is null
            ? Result<ServiceOrderDetailsDTO>.Failure("Orden no encontrada.", "SVC_ORDER_NOT_FOUND", "NotFound")
            : Result<ServiceOrderDetailsDTO>.Success(details);
    }
}

