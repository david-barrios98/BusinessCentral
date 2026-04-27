using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.WorkLogs;

public sealed record CreateWorkLogCommand(int CompanyId, WorkLogDTO WorkLog) : IRequest<Result<long>>;

