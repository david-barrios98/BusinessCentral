using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Bootstrap.Commands;

public sealed class PostConstitutionCapitalHandler : IRequestHandler<PostConstitutionCapitalCommand, Result<long>>
{
    private readonly IPucAccountingRepository _puc;
    private readonly ICompanyFinancialProfileRepository _profile;

    public PostConstitutionCapitalHandler(IPucAccountingRepository puc, ICompanyFinancialProfileRepository profile)
    {
        _puc = puc;
        _profile = profile;
    }

    public async Task<Result<long>> Handle(PostConstitutionCapitalCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<long>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        static decimal R(decimal v) => Math.Round(v, 2, MidpointRounding.AwayFromZero);

        var cash = R(Math.Max(0, request.CashAmount));
        var bank = R(Math.Max(0, request.BankAmount));
        var equip = R(Math.Max(0, request.EquipmentAmount));
        var inv = R(Math.Max(0, request.InventoryAmount));
        var other = R(Math.Max(0, request.OtherAssetsAmount));

        var totalDebit = cash + bank + equip + inv + other;
        if (totalDebit <= 0)
            return Result<long>.Failure("Debe indicar al menos un monto de activo/caja mayor a cero.", "VALIDATION", "Validation");

        if (other > 0 && string.IsNullOrWhiteSpace(request.OtherAssetsAccountCode))
            return Result<long>.Failure("OtherAssetsAccountCode es requerido cuando OtherAssetsAmount > 0.", "VALIDATION", "Validation");

        var capitalCode = string.IsNullOrWhiteSpace(request.CapitalAccountCode) ? "310505" : request.CapitalAccountCode.Trim();
        var cashCode = string.IsNullOrWhiteSpace(request.CashAccountCode) ? "110505" : request.CashAccountCode.Trim();
        var bankCode = string.IsNullOrWhiteSpace(request.BankAccountCode) ? "111005" : request.BankAccountCode.Trim();
        var equipCode = string.IsNullOrWhiteSpace(request.EquipmentAccountCode) ? "152405" : request.EquipmentAccountCode.Trim();
        var invCode = string.IsNullOrWhiteSpace(request.InventoryAccountCode) ? "143505" : request.InventoryAccountCode.Trim();

        async Task<long?> Id(string code) => await _puc.GetAccountIdByCodeAsync(request.CompanyId, code);

        var capitalId = await Id(capitalCode);
        if (capitalId is null)
            return Result<long>.Failure($"No existe cuenta activa con código {capitalCode}.", "PUC_ACCOUNT", "Validation");

        var ops = new List<(decimal amt, string code, long? id, string label)>();
        if (cash > 0)
        {
            var id = await Id(cashCode);
            if (id is null) return Result<long>.Failure($"No existe cuenta activa con código {cashCode}.", "PUC_ACCOUNT", "Validation");
            ops.Add((cash, cashCode, id, "Caja/equivalentes"));
        }
        if (bank > 0)
        {
            var id = await Id(bankCode);
            if (id is null) return Result<long>.Failure($"No existe cuenta activa con código {bankCode}.", "PUC_ACCOUNT", "Validation");
            ops.Add((bank, bankCode, id, "Bancos"));
        }
        if (equip > 0)
        {
            var id = await Id(equipCode);
            if (id is null) return Result<long>.Failure($"No existe cuenta activa con código {equipCode}.", "PUC_ACCOUNT", "Validation");
            ops.Add((equip, equipCode, id, "Equipos"));
        }
        if (inv > 0)
        {
            var id = await Id(invCode);
            if (id is null) return Result<long>.Failure($"No existe cuenta activa con código {invCode}.", "PUC_ACCOUNT", "Validation");
            ops.Add((inv, invCode, id, "Inventarios"));
        }
        if (other > 0)
        {
            var oc = request.OtherAssetsAccountCode!.Trim();
            var id = await Id(oc);
            if (id is null) return Result<long>.Failure($"No existe cuenta activa con código {oc}.", "PUC_ACCOUNT", "Validation");
            ops.Add((other, oc, id, "Otros activos"));
        }

        var jeId = await _puc.CreateJournalEntryAsync(
            request.CompanyId,
            request.EntryDateUtc,
            "OPENING_CONSTITUTION",
            "COMPANY_BOOTSTRAP",
            "CONSTITUTION",
            "Constitución / capital inicial de socios",
            request.PerformedByUserId);

        foreach (var o in ops)
        {
            await _puc.AddJournalEntryLineAsync(
                request.CompanyId,
                jeId,
                o.id!.Value,
                o.amt,
                0,
                null,
                null,
                o.label);
        }

        await _puc.AddJournalEntryLineAsync(
            request.CompanyId,
            jeId,
            capitalId.Value,
            0,
            totalDebit,
            null,
            null,
            "Capital / aportes (contrapartida)");

        var posted = await _puc.PostJournalEntryAsync(request.CompanyId, jeId);
        if (!posted)
            return Result<long>.Failure("No se pudo postear el asiento (revise cuadre o estado).", "POST_JE", "Conflict");

        await _profile.UpdateAsync(
            request.CompanyId,
            null,
            null,
            "IN_PROGRESS",
            null);

        return Result<long>.Success(jeId);
    }
}
