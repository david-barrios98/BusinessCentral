namespace BusinessCentral.Application.DTOs.Finance;

/// <summary>Verificación simple Activo ≈ Pasivo + Patrimonio según reporte PUC clase 1-3.</summary>
public sealed class BalanceEquationCheckDTO
{
    public DateTime AsOfUtc { get; set; }
    public decimal ActivoBalance { get; set; }
    public decimal PasivoBalance { get; set; }
    public decimal PatrimonioBalance { get; set; }
    /// <summary>Diferencia según convención del reporte (Activo - (Pasivo + Patrimonio)).</summary>
    public decimal Difference { get; set; }
    public bool IsBalanced { get; set; }
    public string Message { get; set; } = string.Empty;
}
