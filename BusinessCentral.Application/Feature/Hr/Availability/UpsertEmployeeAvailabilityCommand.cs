using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.Availability;

public sealed record UpsertEmployeeAvailabilityCommand(int CompanyId, EmployeeAvailabilityDTO Availability)
    : IRequest<Result<bool>>;

