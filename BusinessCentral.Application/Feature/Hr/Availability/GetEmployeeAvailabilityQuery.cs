using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.Availability;

public sealed record GetEmployeeAvailabilityQuery(int CompanyId, int UserId)
    : IRequest<Result<EmployeeAvailabilityDTO>>;

