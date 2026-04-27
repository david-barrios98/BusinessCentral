using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Business.StorageLocations.Commands;

public sealed class UpsertStorageLocationHandler : IRequestHandler<UpsertStorageLocationCommand, Result<long>>
{
    private readonly IStorageLocationRepository _repo;

    public UpsertStorageLocationHandler(IStorageLocationRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<long>> Handle(UpsertStorageLocationCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<long>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.Name))
            return Result<long>.Failure("Code/Name requeridos.", "VALIDATION", "Validation");

        if (string.IsNullOrWhiteSpace(request.Type))
            return Result<long>.Failure("Type requerido.", "VALIDATION", "Validation");

        var id = await _repo.UpsertAsync(
            request.CompanyId,
            request.Id,
            request.FacilityId,
            request.Code.Trim(),
            request.Name.Trim(),
            request.Type.Trim().ToUpperInvariant(),
            request.ParentLocationId,
            request.Notes,
            request.Active,
            request.PerformedByUserId
        );

        return Result<long>.Success(id);
    }
}

