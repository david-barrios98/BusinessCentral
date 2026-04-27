using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.Employees;

public sealed record GetEmployeeProfileQuery(int CompanyId, int UserId) : IRequest<Result<EmployeeProfileDTO>>;

