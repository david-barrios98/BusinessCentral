using Microsoft.Extensions.Caching.Memory;
using System;


namespace BusinessCentral.Infrastructure.Security
{
    public class BlackListService
    {
        private readonly IMemoryCache _cache;

        public BlackListService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void AddToBlacklist(string token, DateTime? expiration)
        {
            _cache.Set(token, true, (DateTimeOffset)expiration);
        }

        public bool IsTokenBlacklisted(string token)
        {
            return _cache.TryGetValue(token, out _);
        }
    }
}
