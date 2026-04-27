using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Manufacturing.Commands;

public sealed class PostProductionBatchHandler : IRequestHandler<PostProductionBatchCommand, Result<bool>>
{
    private readonly IManufacturingRepository _repo;

    public PostProductionBatchHandler(IManufacturingRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<bool>> Handle(PostProductionBatchCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0 || request.BatchId <= 0)
            return Result<bool>.Failure("CompanyId/BatchId inválido.", "VALIDATION", "Validation");

        var ok = await _repo.PostBatchAsync(request.CompanyId, request.BatchId);
        return Result<bool>.Success(ok);
    }
}

