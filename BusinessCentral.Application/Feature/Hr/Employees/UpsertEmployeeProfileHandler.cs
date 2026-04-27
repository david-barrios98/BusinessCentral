using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.Employees;

public sealed class UpsertEmployeeProfileHandler : IRequestHandler<UpsertEmployeeProfileCommand, Result<bool>>
{
    private readonly IHrRepository _hr;

    public UpsertEmployeeProfileHandler(IHrRepository hr)
    {
        _hr = hr;
    }

    public async Task<Result<bool>> Handle(UpsertEmployeeProfileCommand request, CancellationToken cancellationToken)
    {
        var ok = await _hr.UpsertEmployeeProfileAsync(request.CompanyId, request.Profile);
        return ok
            ? Result<bool>.Success(true)
            : Result<bool>.Failure("No se pudo guardar el perfil del empleado.", "HR_PROFILE_UPSERT_FAILED", "Conflict");
    }
}

