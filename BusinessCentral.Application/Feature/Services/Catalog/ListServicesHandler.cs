using BusinessCentral.Application.DTOs.Services;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Services.Catalog;

public sealed class ListServicesHandler : IRequestHandler<ListServicesQuery, Result<List<ServiceDTO>>>
{
    private readonly IServicesRepository _services;

    public ListServicesHandler(IServicesRepository services)
    {
        _services = services;
    }

    public async Task<Result<List<ServiceDTO>>> Handle(ListServicesQuery request, CancellationToken cancellationToken)
    {
        var list = await _services.ListServicesAsync(request.CompanyId, request.OnlyActive);
        return Result<List<ServiceDTO>>.Success(list);
    }
}

