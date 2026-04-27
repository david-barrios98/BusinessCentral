using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Bootstrap.Commands;

/// <summary>
/// Asiento tipo constitución: débitos en caja/bancos/activos vs crédito a patrimonio (capital).
/// Códigos PUC por defecto orientados a PUC empresarial Colombia; sobrescribibles.
/// </summary>
public sealed record PostConstitutionCapitalCommand(
    int CompanyId,
    DateTime EntryDateUtc,
    decimal CashAmount,
    decimal BankAmount,
    decimal EquipmentAmount,
    decimal InventoryAmount,
    decimal OtherAssetsAmount,
    string? OtherAssetsAccountCode,
    string? CapitalAccountCode,
    string? CashAccountCode,
    string? BankAccountCode,
    string? EquipmentAccountCode,
    string? InventoryAccountCode,
    int? PerformedByUserId
) : IRequest<Result<long>>;
