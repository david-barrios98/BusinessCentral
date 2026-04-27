using BusinessCentral.Application.DTOs.Commerce;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Suppliers.Queries;

public sealed class ListSuppliersHandler : IRequestHandler<ListSuppliersQuery, Result<List<SupplierDTO>>>
{
    private readonly ISupplierRepository _repo;

    public ListSuppliersHandler(ISupplierRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<List<SupplierDTO>>> Handle(ListSuppliersQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<List<SupplierDTO>>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        var data = await _repo.ListAsync(request.CompanyId, request.OnlyActive, request.Q);
        return Result<List<SupplierDTO>>.Success(data);
    }
}

