using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Services.Orders;

public sealed class AddServiceOrderLineHandler : IRequestHandler<AddServiceOrderLineCommand, Result<long>>
{
    private readonly IServicesRepository _services;

    public AddServiceOrderLineHandler(IServicesRepository services)
    {
        _services = services;
    }

    public async Task<Result<long>> Handle(AddServiceOrderLineCommand request, CancellationToken cancellationToken)
    {
        if (request.OrderId <= 0 || request.ServiceId <= 0 || request.Quantity <= 0 || request.UnitPrice < 0)
            return Result<long>.Failure("Datos inválidos para la línea.", "SVC_ORDERLINE_VALIDATION", "Validation");

        var id = await _services.AddServiceOrderLineAsync(
            request.CompanyId,
            request.OrderId,
            request.ServiceId,
            request.Quantity,
            request.UnitPrice,
            request.EmployeeUserId
        );

        return id > 0
            ? Result<long>.Success(id)
            : Result<long>.Failure("No se pudo agregar la línea.", "SVC_ORDERLINE_CREATE_FAILED", "Conflict");
    }
}

