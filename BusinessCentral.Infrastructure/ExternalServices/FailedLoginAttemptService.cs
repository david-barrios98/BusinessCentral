using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using BusinessCentral.Application.Ports.Outbound;

namespace BusinessCentral.Infrastructure.ExternalServices;

/// <summary>
/// Adaptador: Implementaci�n de prevenci�n de fuerza bruta
/// Usa cach� en memoria (escalable a Redis)
/// </summary>
public class FailedLoginAttemptService : IFailedLoginAttemptService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<FailedLoginAttemptService> _logger;
    private const int MaxFailedAttempts = 5;
    private const int LockoutDurationMinutes = 15;
    private const string CacheKeyPrefix = "failed_login_";

    public FailedLoginAttemptService(IMemoryCache cache, ILogger<FailedLoginAttemptService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task RecordFailedAttemptAsync(string username)
    {
        var key = $"{CacheKeyPrefix}{username}";
        if (!_cache.TryGetValue(key, out int attempts))
        {
            attempts = 0;
        }
        attempts++;

        _cache.Set(key, attempts, TimeSpan.FromMinutes(LockoutDurationMinutes));

        _logger.LogWarning("Intento fallido de login #{Attempts} para usuario: {username}",
            attempts, username);

        if (attempts >= MaxFailedAttempts)
        {
            _logger.LogWarning("Cuenta bloqueada por m�ltiples intentos fallidos: {username}", username);
        }

        await Task.CompletedTask;
    }

    public async Task<bool> IsAccountLockedAsync(string username)
    {
        var key = $"{CacheKeyPrefix}{username}";
        if (!_cache.TryGetValue(key, out int attempts))
        {
            attempts = 0;
        }
        return await Task.FromResult(attempts >= MaxFailedAttempts);
    }

    public async Task ClearFailedAttemptsAsync(string username)
    {
        var key = $"{CacheKeyPrefix}{username}";
        _cache.Remove(key);
        await Task.CompletedTask;
    }
}