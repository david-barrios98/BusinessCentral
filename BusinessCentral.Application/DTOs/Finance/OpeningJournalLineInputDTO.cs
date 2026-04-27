namespace BusinessCentral.Application.DTOs.Finance;

/// <summary>Línea de asiento de apertura por código PUC (auxiliar).</summary>
public sealed class OpeningJournalLineInputDTO
{
    /// <summary>Código cuenta PUC existente para la compañía (ej: 130505).</summary>
    public string AccountCode { get; set; } = string.Empty;

    public decimal Debit { get; set; }
    public decimal Credit { get; set; }

    /// <summary>NIT/CC del tercero cuando la cuenta lo exige (cartera, proveedores).</summary>
    public string? ThirdPartyDocument { get; set; }

    public string? ThirdPartyName { get; set; }
    public string? Notes { get; set; }
}
