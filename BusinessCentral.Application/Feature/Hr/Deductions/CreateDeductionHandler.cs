using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.Deductions;

public sealed class CreateDeductionHandler : IRequestHandler<CreateDeductionCommand, Result<long>>
{
    private readonly IHrRepository _hr;

    public CreateDeductionHandler(IHrRepository hr)
    {
        _hr = hr;
    }

    public async Task<Result<long>> Handle(CreateDeductionCommand request, CancellationToken cancellationToken)
    {
        if (request.Deduction.UserId <= 0 || request.Deduction.Amount <= 0)
            return Result<long>.Failure("UserId y Amount son requeridos.", "HR_DEDUCTION_VALIDATION", "Validation");

        var id = await _hr.CreateDeductionAsync(request.CompanyId, request.Deduction);
        return id > 0
            ? Result<long>.Success(id)
            : Result<long>.Failure("No se pudo crear la deducción.", "HR_DEDUCTION_CREATE_FAILED", "Conflict");
    }
}

