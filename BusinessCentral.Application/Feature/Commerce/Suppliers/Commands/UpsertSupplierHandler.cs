using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Commerce.Suppliers.Commands;

public sealed class UpsertSupplierHandler : IRequestHandler<UpsertSupplierCommand, Result<long>>
{
    private readonly ISupplierRepository _repo;

    public UpsertSupplierHandler(ISupplierRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<long>> Handle(UpsertSupplierCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<long>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<long>.Failure("Name requerido.", "VALIDATION", "Validation");

        var id = await _repo.UpsertAsync(
            request.CompanyId,
            request.Id,
            request.Name.Trim(),
            request.DocumentNumber,
            request.Phone,
            request.Email,
            request.Notes,
            request.Active
        );

        return Result<long>.Success(id);
    }
}

