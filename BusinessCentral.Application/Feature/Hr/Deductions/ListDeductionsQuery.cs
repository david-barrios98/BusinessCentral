using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.Deductions;

public sealed record ListDeductionsQuery(int CompanyId, int? UserId, string? Type) : IRequest<Result<List<DeductionDTO>>>;

