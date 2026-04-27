using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Hr.PaySchemes;

public sealed class UpsertPaySchemeHandler : IRequestHandler<UpsertPaySchemeCommand, Result<bool>>
{
    private readonly IHrRepository _hr;

    public UpsertPaySchemeHandler(IHrRepository hr)
    {
        _hr = hr;
    }

    public async Task<Result<bool>> Handle(UpsertPaySchemeCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.Name))
            return Result<bool>.Failure("Code y Name son requeridos.", "HR_PAYSCHEME_VALIDATION", "Validation");

        var ok = await _hr.UpsertPaySchemeAsync(request.CompanyId, request.Code, request.Name, request.Unit, request.Active);
        return ok
            ? Result<bool>.Success(true)
            : Result<bool>.Failure("No se pudo guardar el esquema de pago.", "HR_PAYSCHEME_UPSERT_FAILED", "Conflict");
    }
}

