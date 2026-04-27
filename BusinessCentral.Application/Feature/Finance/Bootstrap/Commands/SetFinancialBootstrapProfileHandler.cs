using BusinessCentral.Application.Constants;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Bootstrap.Commands;

public sealed class SetFinancialBootstrapProfileHandler : IRequestHandler<SetFinancialBootstrapProfileCommand, Result<bool>>
{
    private readonly ICompanyFinancialProfileRepository _repo;

    public SetFinancialBootstrapProfileHandler(ICompanyFinancialProfileRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<bool>> Handle(SetFinancialBootstrapProfileCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<bool>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        if (!FinancialBootstrapStatuses.IsKnown(request.BootstrapStatus))
            return Result<bool>.Failure("BootstrapStatus inválido.", "VALIDATION", "Validation");

        if (request.StartupMode is not null && !FinancialStartupModes.IsKnown(request.StartupMode))
            return Result<bool>.Failure("StartupMode inválido. Use CONSTITUTION, SANITATION o MIGRATION.", "VALIDATION", "Validation");

        var ok = await _repo.UpdateAsync(
            request.CompanyId,
            request.StartupMode,
            request.OperatingStartDateUtc,
            request.BootstrapStatus,
            request.Notes);

        return ok
            ? Result<bool>.Success(true)
            : Result<bool>.Failure("No se pudo actualizar el perfil financiero.", "BOOTSTRAP_UPDATE", "Conflict");
    }
}
