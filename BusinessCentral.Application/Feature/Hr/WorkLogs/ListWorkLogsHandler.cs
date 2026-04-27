using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.WorkLogs;

public sealed class ListWorkLogsHandler : IRequestHandler<ListWorkLogsQuery, Result<List<WorkLogDTO>>>
{
    private readonly IHrRepository _hr;

    public ListWorkLogsHandler(IHrRepository hr)
    {
        _hr = hr;
    }

    public async Task<Result<List<WorkLogDTO>>> Handle(ListWorkLogsQuery request, CancellationToken cancellationToken)
    {
        var list = await _hr.ListWorkLogsAsync(request.CompanyId, request.UserId, request.FromDate, request.ToDate);
        return Result<List<WorkLogDTO>>.Success(list);
    }
}

