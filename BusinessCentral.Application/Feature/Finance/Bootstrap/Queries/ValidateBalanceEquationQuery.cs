using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Bootstrap.Queries;

/// <summary>
/// Verifica que la suma de balances netos de clases 1+2+3 sea ~0 según el reporte de balance PUC publicado.
/// Útil tras importar saldos de migración.
/// </summary>
public sealed record ValidateBalanceEquationQuery(int CompanyId, DateTime AsOfUtc)
    : IRequest<Result<BalanceEquationCheckDTO>>;
