using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.Employees;

public sealed record UpsertEmployeeProfileCommand(int CompanyId, EmployeeProfileDTO Profile) : IRequest<Result<bool>>;

