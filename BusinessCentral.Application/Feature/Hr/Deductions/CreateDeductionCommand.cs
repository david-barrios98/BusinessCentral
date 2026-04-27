using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.Deductions;

public sealed record CreateDeductionCommand(int CompanyId, DeductionDTO Deduction) : IRequest<Result<long>>;

