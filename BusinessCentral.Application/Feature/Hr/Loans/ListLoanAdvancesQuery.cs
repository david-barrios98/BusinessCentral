using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.Loans;

public sealed record ListLoanAdvancesQuery(int CompanyId, int? UserId, string? Status) : IRequest<Result<List<LoanAdvanceDTO>>>;

