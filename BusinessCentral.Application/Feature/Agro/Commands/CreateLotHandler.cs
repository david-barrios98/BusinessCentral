using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Agro.Commands;

public sealed class CreateLotHandler : IRequestHandler<CreateLotCommand, Result<long>>
{
    private readonly IAgroRepository _repo;

    public CreateLotHandler(IAgroRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<long>> Handle(CreateLotCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0 || string.IsNullOrWhiteSpace(request.Kind) || string.IsNullOrWhiteSpace(request.Code))
            return Result<long>.Failure("CompanyId/Kind/Code inválido.", "VALIDATION", "Validation");

        var id = await _repo.CreateLotAsync(
            request.CompanyId,
            request.Kind.Trim().ToUpperInvariant(),
            request.Code.Trim(),
            request.Name,
            request.StartDate,
            request.InitialUnits,
            request.InitialAvgWeightKg,
            request.Notes,
            request.CreatedByUserId
        );

        return Result<long>.Success(id);
    }
}

