using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Bootstrap.Commands;

/// <summary>
/// Asiento de apertura libre (saneamiento o migración). Debe cuadrar débitos = créditos.
/// </summary>
public sealed record PostOpeningBalancesJournalCommand(
    int CompanyId,
    DateTime EntryDateUtc,
    /// <summary>SANITATION o MIGRATION (texto libre corto para referencia).</summary>
    string OpeningKind,
    IReadOnlyList<OpeningJournalLineInputDTO> Lines,
    string? Description,
    int? PerformedByUserId
) : IRequest<Result<long>>;
