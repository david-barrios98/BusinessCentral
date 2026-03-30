namespace BusinessCentral.Application.Ports.Outbound;

/// <summary>
/// Puerto de salida: Servicio para prevenir ataques de fuerza bruta
/// </summary>
public interface IFailedLoginAttemptService
{
    Task RecordFailedAttemptAsync(string username);
    Task<bool> IsAccountLockedAsync(string username);
    Task ClearFailedAttemptsAsync(string username);
}