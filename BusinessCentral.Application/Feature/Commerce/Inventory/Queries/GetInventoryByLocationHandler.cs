using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Inventory.Queries;

public sealed class GetInventoryByLocationHandler : IRequestHandler<GetInventoryByLocationQuery, Result<List<InventoryByLocationRowDTO>>>
{
    private readonly IInventoryLocationReportRepository _repo;

    public GetInventoryByLocationHandler(IInventoryLocationReportRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<List<InventoryByLocationRowDTO>>> Handle(GetInventoryByLocationQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<List<InventoryByLocationRowDTO>>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        var data = await _repo.GetInventoryByLocationAsync(request.CompanyId, request.AsOfUtc, request.LocationId);
        return Result<List<InventoryByLocationRowDTO>>.Success(data);
    }
}

