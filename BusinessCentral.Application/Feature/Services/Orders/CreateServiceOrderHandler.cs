using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Services.Orders;

public sealed class CreateServiceOrderHandler : IRequestHandler<CreateServiceOrderCommand, Result<long>>
{
    private readonly IServicesRepository _services;

    public CreateServiceOrderHandler(IServicesRepository services)
    {
        _services = services;
    }

    public async Task<Result<long>> Handle(CreateServiceOrderCommand request, CancellationToken cancellationToken)
    {
        var id = await _services.CreateServiceOrderAsync(request.CompanyId, request.VehicleType, request.Plate, request.CustomerName);
        return id > 0
            ? Result<long>.Success(id)
            : Result<long>.Failure("No se pudo crear la orden.", "SVC_ORDER_CREATE_FAILED", "Conflict");
    }
}

