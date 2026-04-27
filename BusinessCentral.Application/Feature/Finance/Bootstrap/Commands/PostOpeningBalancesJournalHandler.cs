using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Bootstrap.Commands;

public sealed class PostOpeningBalancesJournalHandler : IRequestHandler<PostOpeningBalancesJournalCommand, Result<long>>
{
    private const decimal Tolerance = 0.009m;

    private readonly IPucAccountingRepository _puc;
    private readonly ICompanyFinancialProfileRepository _profile;

    public PostOpeningBalancesJournalHandler(IPucAccountingRepository puc, ICompanyFinancialProfileRepository profile)
    {
        _puc = puc;
        _profile = profile;
    }

    public async Task<Result<long>> Handle(PostOpeningBalancesJournalCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<long>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        if (request.Lines is null || request.Lines.Count == 0)
            return Result<long>.Failure("Debe enviar al menos una línea.", "VALIDATION", "Validation");

        decimal sumD = 0, sumC = 0;
        foreach (var line in request.Lines)
        {
            var d = Math.Round(Math.Max(0, line.Debit), 2, MidpointRounding.AwayFromZero);
            var c = Math.Round(Math.Max(0, line.Credit), 2, MidpointRounding.AwayFromZero);
            if (d > 0 && c > 0)
                return Result<long>.Failure($"La cuenta {line.AccountCode} no puede tener débito y crédito positivos a la vez.", "VALIDATION", "Validation");
            if (string.IsNullOrWhiteSpace(line.AccountCode))
                return Result<long>.Failure("AccountCode requerido en cada línea.", "VALIDATION", "Validation");
            sumD += d;
            sumC += c;
        }

        if (Math.Abs(sumD - sumC) > Tolerance)
            return Result<long>.Failure($"El asiento no cuadra: total débitos {sumD:N2} vs créditos {sumC:N2}.", "NOT_BALANCED", "Validation");

        if (sumD <= Tolerance)
            return Result<long>.Failure("Montos en cero: no hay asiento válido.", "VALIDATION", "Validation");

        var kind = string.IsNullOrWhiteSpace(request.OpeningKind) ? "OPENING" : request.OpeningKind.Trim().ToUpperInvariant();
        var desc = string.IsNullOrWhiteSpace(request.Description)
            ? $"Apertura de saldos ({kind})"
            : request.Description!.Trim();

        var jeId = await _puc.CreateJournalEntryAsync(
            request.CompanyId,
            request.EntryDateUtc,
            "OPENING_BALANCE",
            "COMPANY_BOOTSTRAP",
            kind,
            desc,
            request.PerformedByUserId);

        foreach (var line in request.Lines)
        {
            var d = Math.Round(Math.Max(0, line.Debit), 2, MidpointRounding.AwayFromZero);
            var c = Math.Round(Math.Max(0, line.Credit), 2, MidpointRounding.AwayFromZero);
            if (d <= 0 && c <= 0)
                continue;

            var code = line.AccountCode.Trim();
            var accId = await _puc.GetAccountIdByCodeAsync(request.CompanyId, code);
            if (accId is null)
                return Result<long>.Failure($"Cuenta no encontrada o inactiva: {code}.", "PUC_ACCOUNT", "Validation");

            await _puc.AddJournalEntryLineAsync(
                request.CompanyId,
                jeId,
                accId.Value,
                d,
                c,
                string.IsNullOrWhiteSpace(line.ThirdPartyDocument) ? null : line.ThirdPartyDocument.Trim(),
                string.IsNullOrWhiteSpace(line.ThirdPartyName) ? null : line.ThirdPartyName.Trim(),
                line.Notes);
        }

        var posted = await _puc.PostJournalEntryAsync(request.CompanyId, jeId);
        if (!posted)
            return Result<long>.Failure("No se pudo postear el asiento.", "POST_JE", "Conflict");

        await _profile.UpdateAsync(request.CompanyId, null, null, "IN_PROGRESS", null);

        return Result<long>.Success(jeId);
    }
}
