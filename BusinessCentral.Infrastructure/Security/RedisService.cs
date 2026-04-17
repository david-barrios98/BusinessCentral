using BusinessCentral.Application.Ports.Outbound;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Infrastructure.Security
{
    public class RedisService: IRedisService
    {
        private readonly IDistributedCache _redisCache; // Usamos la interfaz distribuida
        private readonly ILogger<RedisService> _logger;

        public RedisService(IDistributedCache redisCache, ILogger<RedisService> logger)
        {
            _redisCache = redisCache;
            _logger = logger;
        }

        public async Task RevokeTokenAsync(string jti, TimeSpan expiration)
        {
            // Guardamos el JTI en Redis. Si el JTI está presente, el token no sirve.
            await _redisCache.SetStringAsync(
                $"revoked_token:{jti}",
                "true",
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration }
            );
            _logger.LogWarning("Token JTI {Jti} ha sido revocado permanentemente.", jti);
        }

        public async Task<bool> IsTokenRevokedAsync(string jti)
        {
            var result = await _redisCache.GetStringAsync($"revoked_token:{jti}");
            return result != null;
        }
    }
}
