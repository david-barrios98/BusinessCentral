using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.Employees;

public sealed class GetEmployeeProfileHandler : IRequestHandler<GetEmployeeProfileQuery, Result<EmployeeProfileDTO>>
{
    private readonly IHrRepository _hr;

    public GetEmployeeProfileHandler(IHrRepository hr)
    {
        _hr = hr;
    }

    public async Task<Result<EmployeeProfileDTO>> Handle(GetEmployeeProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _hr.GetEmployeeProfileAsync(request.CompanyId, request.UserId);
        return profile is null
            ? Result<EmployeeProfileDTO>.Failure("Empleado no encontrado.", "HR_EMPLOYEE_NOT_FOUND", "NotFound")
            : Result<EmployeeProfileDTO>.Success(profile);
    }
}

