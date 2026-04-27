using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Bootstrap.Commands;

public sealed record SetFinancialBootstrapProfileCommand(
    int CompanyId,
    /// <summary>CONSTITUTION | SANITATION | MIGRATION o null para no cambiar.</summary>
    string? StartupMode,
    DateTime? OperatingStartDateUtc,
    /// <summary>NOT_STARTED | IN_PROGRESS | COMPLETED</summary>
    string BootstrapStatus,
    string? Notes
) : IRequest<Result<bool>>;
