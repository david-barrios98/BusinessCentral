using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Services.Catalog;

public sealed class UpsertServiceHandler : IRequestHandler<UpsertServiceCommand, Result<bool>>
{
    private readonly IServicesRepository _services;

    public UpsertServiceHandler(IServicesRepository services)
    {
        _services = services;
    }

    public async Task<Result<bool>> Handle(UpsertServiceCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.Name))
            return Result<bool>.Failure("Code y Name son requeridos.", "SVC_SERVICE_VALIDATION", "Validation");

        var ok = await _services.UpsertServiceAsync(request.CompanyId, request.Code, request.Name, request.BasePrice, request.Active);
        return ok
            ? Result<bool>.Success(true)
            : Result<bool>.Failure("No se pudo guardar el servicio.", "SVC_SERVICE_UPSERT_FAILED", "Conflict");
    }
}

