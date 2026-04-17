using BusinessCentral.Shared.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BusinessCentral.Infrastructure.Security
{
    public class MemoryCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheService> _logger;

        // Tiempo por defecto si no se especifica uno
        private readonly TimeSpan _defaultExpiration = TimeZoneHelper.GetColombiaTimeNow().AddMinutes(30).TimeOfDay;

        public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Guarda un objeto en caché con un tipo genérico T.
        /// </summary>
        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration,
                // Priority asegura que no se borre de la RAM agresivamente 
                Priority = CacheItemPriority.High
            };

            _cache.Set(key, value, options);
            _logger.LogDebug("Cache entry set for key: {Key}", key);
        }

        /// <summary>
        /// Obtiene un valor. Si no existe, devuelve el valor por defecto del tipo T.
        /// </summary>
        public T? Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out T value))
            {
                return value;
            }

            _logger.LogTrace("Cache miss for key: {Key}", key);
            return default;
        }

        /// <summary>
        /// Patrón Get or Create: Busca el dato y, si no existe, ejecuta una función para traerlo (ej. de DB) y guardarlo.
        /// </summary>
        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            if (!_cache.TryGetValue(key, out T result))
            {
                _logger.LogInformation("Cache miss for {Key}. Fetching from source...", key);
                result = await factory();
                Set(key, result, expiration);
            }
            return result;
        }

        /// <summary>
        /// Elimina una entrada. Fundamental para cuando los datos cambian en la base de datos.
        /// </summary>
        public void Remove(string key)
        {
            _cache.Remove(key);
            _logger.LogInformation("Cache entry removed for key: {Key}", key);
        }

        /// <summary>
        /// Valida si existe una llave.
        /// </summary>
        public bool Exists(string key)
        {
            return _cache.TryGetValue(key, out _);
        }
    }
}