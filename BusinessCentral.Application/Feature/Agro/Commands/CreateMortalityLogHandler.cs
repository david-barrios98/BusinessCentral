using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Agro.Commands;

public sealed class CreateMortalityLogHandler : IRequestHandler<CreateMortalityLogCommand, Result<long>>
{
    private readonly IAgroRepository _repo;

    public CreateMortalityLogHandler(IAgroRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<long>> Handle(CreateMortalityLogCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0 || request.LotId <= 0)
            return Result<long>.Failure("Parámetros inválidos.", "VALIDATION", "Validation");

        if (request.Units <= 0)
            return Result<long>.Failure("Units inválidas.", "VALIDATION", "Validation");

        var id = await _repo.CreateMortalityLogAsync(
            request.CompanyId,
            request.LotId,
            request.MortalityDate,
            request.Units,
            request.AvgWeightKg,
            request.Notes
        );

        return Result<long>.Success(id);
    }
}

