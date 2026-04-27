using BusinessCentral.Application.DTOs.Hr;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.Loans;

public sealed record CreateLoanAdvanceCommand(int CompanyId, LoanAdvanceDTO Loan) : IRequest<Result<long>>;

