using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Manufacturing.Commands;

public sealed record PostProductionBatchCommand(int CompanyId, long BatchId)
    : IRequest<Result<bool>>;

