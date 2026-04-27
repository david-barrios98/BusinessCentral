using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Bootstrap.Queries;

public sealed class ValidateBalanceEquationHandler : IRequestHandler<ValidateBalanceEquationQuery, Result<BalanceEquationCheckDTO>>
{
    private const decimal Tol = 5m; // pesos: tolerancia por redondeos / cuentas fuera de 1-3

    private readonly IPucAccountingRepository _puc;

    public ValidateBalanceEquationHandler(IPucAccountingRepository puc)
    {
        _puc = puc;
    }

    public async Task<Result<BalanceEquationCheckDTO>> Handle(ValidateBalanceEquationQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<BalanceEquationCheckDTO>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        var rows = await _puc.GetBalanceSheetAsync(request.CompanyId, request.AsOfUtc);
        decimal b1 = 0, b2 = 0, b3 = 0;
        foreach (var r in rows)
        {
            switch (r.ClassCode)
            {
                case "1": b1 = r.Balance; break;
                case "2": b2 = r.Balance; break;
                case "3": b3 = r.Balance; break;
            }
        }

        var sum = b1 + b2 + b3;
        var dto = new BalanceEquationCheckDTO
        {
            AsOfUtc = request.AsOfUtc,
            ActivoBalance = b1,
            PasivoBalance = b2,
            PatrimonioBalance = b3,
            Difference = sum,
            IsBalanced = Math.Abs(sum) <= Tol,
            Message = Math.Abs(sum) <= Tol
                ? "Las clases 1–3 cuadran dentro de tolerancia (solo cuentas de balance en el reporte)."
                : $"Diferencia neta en clases 1–3: {sum:N2}. Revise saldos de apertura o cuentas fuera de rango."
        };

        return Result<BalanceEquationCheckDTO>.Success(dto);
    }
}
