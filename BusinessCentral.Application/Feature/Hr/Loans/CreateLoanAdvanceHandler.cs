using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.Loans;

public sealed class CreateLoanAdvanceHandler : IRequestHandler<CreateLoanAdvanceCommand, Result<long>>
{
    private readonly IHrRepository _hr;

    public CreateLoanAdvanceHandler(IHrRepository hr)
    {
        _hr = hr;
    }

    public async Task<Result<long>> Handle(CreateLoanAdvanceCommand request, CancellationToken cancellationToken)
    {
        if (request.Loan.UserId <= 0 || request.Loan.Amount <= 0)
            return Result<long>.Failure("UserId y Amount son requeridos.", "HR_LOAN_VALIDATION", "Validation");

        var id = await _hr.CreateLoanAdvanceAsync(request.CompanyId, request.Loan);
        return id > 0
            ? Result<long>.Success(id)
            : Result<long>.Failure("No se pudo crear el préstamo/avance.", "HR_LOAN_CREATE_FAILED", "Conflict");
    }
}

