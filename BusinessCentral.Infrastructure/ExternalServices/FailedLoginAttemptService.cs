using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Persistence.Configuration;

namespace BusinessCentral.Infrastructure.ExternalServices;

public class FailedLoginAttemptService : IFailedLoginAttemptService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<FailedLoginAttemptService> _logger;
    private readonly FailedLoginOptions _options;

    public FailedLoginAttemptService(
        IMemoryCache cache,
        ILogger<FailedLoginAttemptService> logger,
        IOptions<FailedLoginOptions> options)
    {
        _cache = cache;
        _logger = logger;
        _options = options?.Value ?? new FailedLoginOptions();
    }

    public Task RecordFailedAttemptAsync(string username)
    {
        var key = $"{_options.CacheKeyPrefix}{username}";
        if (!_cache.TryGetValue(key, out int attempts))
        {
            attempts = 0;
        }
        attempts++;

        _cache.Set(key, attempts, TimeSpan.FromMinutes(_options.LockoutDurationMinutes));

        _logger.LogWarning("Intento fallido de login #{Attempts} para usuario: {UserName}",
            attempts, username);

        if (attempts >= _options.MaxFailedAttempts)
        {
            _logger.LogWarning("Cuenta bloqueada por múltiples intentos fallidos: {UserName}", username);
        }

        return Task.CompletedTask;
    }

    public Task<bool> IsAccountLockedAsync(string username)
    {
        var key = $"{_options.CacheKeyPrefix}{username}";
        if (!_cache.TryGetValue(key, out int attempts))
        {
            attempts = 0;
        }
        return Task.FromResult(attempts >= _options.MaxFailedAttempts);
    }

    public Task ClearFailedAttemptsAsync(string username)
    {
        var key = $"{_options.CacheKeyPrefix}{username}";
        _cache.Remove(key);
        return Task.CompletedTask;
    }
}