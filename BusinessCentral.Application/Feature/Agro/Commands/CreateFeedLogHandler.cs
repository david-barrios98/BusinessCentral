using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Agro.Commands;

public sealed class CreateFeedLogHandler : IRequestHandler<CreateFeedLogCommand, Result<long>>
{
    private readonly IAgroRepository _repo;

    public CreateFeedLogHandler(IAgroRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<long>> Handle(CreateFeedLogCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0 || request.LotId <= 0 || request.FeedProductId <= 0)
            return Result<long>.Failure("Parámetros inválidos.", "VALIDATION", "Validation");

        if (request.Quantity <= 0)
            return Result<long>.Failure("Quantity inválida.", "VALIDATION", "Validation");

        var id = await _repo.CreateFeedLogAsync(
            request.CompanyId,
            request.LotId,
            request.FeedDate,
            request.FeedProductId,
            request.FeedVariantId,
            request.Quantity,
            request.FromLocationId,
            request.UnitCost,
            request.Notes
        );

        return Result<long>.Success(id);
    }
}

