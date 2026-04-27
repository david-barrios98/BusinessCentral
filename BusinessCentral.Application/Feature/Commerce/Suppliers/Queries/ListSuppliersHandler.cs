using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Suppliers.Queries;

public sealed class ListSuppliersHandler : IRequestHandler<ListSuppliersQuery, Result<PagedResult<SupplierDTO>>>
{
    private readonly ISupplierRepository _repo;

    public ListSuppliersHandler(ISupplierRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<PagedResult<SupplierDTO>>> Handle(ListSuppliersQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<PagedResult<SupplierDTO>>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 500 ? 50 : request.PageSize;

        var data = await _repo.ListAsync(request.CompanyId, request.OnlyActive, page, pageSize, request.Q);
        return Result<PagedResult<SupplierDTO>>.Success(data);
    }
}

