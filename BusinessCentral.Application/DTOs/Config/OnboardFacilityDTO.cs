namespace BusinessCentral.Application.DTOs.Config;

/// <summary>
/// Sede / punto de operación en el alta de compañía (serializado a JSON para el SP).
/// </summary>
public sealed class OnboardFacilityDTO
{
    public int FacilityTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    /// <summary>Si es null, el SP asigna prioridad por orden del arreglo.</summary>
    public int? Priority { get; set; }
}
