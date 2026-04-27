using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.WorkLogs;

public sealed record ListWorkLogsQuery(
    int CompanyId,
    int? UserId,
    DateTime? FromDate,
    DateTime? ToDate
) : IRequest<Result<List<WorkLogDTO>>>;

