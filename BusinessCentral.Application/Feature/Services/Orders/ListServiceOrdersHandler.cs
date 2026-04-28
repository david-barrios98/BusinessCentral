using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.DTOs.Services;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Services.Orders;

public sealed class ListServiceOrdersHandler
    : IRequestHandler<ListServiceOrdersQuery, Result<PagedResult<ServiceOrderDTO>>>
{
    private readonly IServicesRepository _repo;

    public ListServiceOrdersHandler(IServicesRepository repo) => _repo = repo;

    public async Task<Result<PagedResult<ServiceOrderDTO>>> Handle(
        ListServiceOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var p = await _repo.ListServiceOrdersAsync(
            request.CompanyId,
            request.Status,
            request.Page,
            request.PageSize);
        return Result<PagedResult<ServiceOrderDTO>>.Success(p);
    }
}
