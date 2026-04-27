using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.WorkLogs;

public sealed class CreateWorkLogHandler : IRequestHandler<CreateWorkLogCommand, Result<long>>
{
    private readonly IHrRepository _hr;

    public CreateWorkLogHandler(IHrRepository hr)
    {
        _hr = hr;
    }

    public async Task<Result<long>> Handle(CreateWorkLogCommand request, CancellationToken cancellationToken)
    {
        if (request.WorkLog.UserId <= 0 || request.WorkLog.PaySchemeId <= 0)
            return Result<long>.Failure("UserId y PaySchemeId son requeridos.", "HR_WORKLOG_VALIDATION", "Validation");

        if (request.WorkLog.Quantity <= 0)
            return Result<long>.Failure("Quantity debe ser mayor a 0.", "HR_WORKLOG_VALIDATION", "Validation");

        var id = await _hr.CreateWorkLogAsync(request.CompanyId, request.WorkLog);
        return id > 0
            ? Result<long>.Success(id)
            : Result<long>.Failure("No se pudo crear el registro.", "HR_WORKLOG_CREATE_FAILED", "Conflict");
    }
}

