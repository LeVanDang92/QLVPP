using Microsoft.Extensions.Caching.Memory;
using OSM.Application.Abstractions.Caching;
using System.Collections.Concurrent;

namespace OSM.Infrastructure.Caching
{
    public sealed class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
    {
        private readonly ConcurrentDictionary<string, byte> _keys = new();

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            memoryCache.TryGetValue(key, out T? value);
            return Task.FromResult(value);
        }

        public Task SetAsync<T>(
            string key,
            T value,
            TimeSpan? absoluteExpirationRelativeToNow = null,
            CancellationToken cancellationToken = default)
        {
            var options = new MemoryCacheEntryOptions();

            if (absoluteExpirationRelativeToNow.HasValue)
            {
                options.SetAbsoluteExpiration(absoluteExpirationRelativeToNow.Value);
            }

            options.RegisterPostEvictionCallback((cacheKey, _, _, _) =>
            {
                if (cacheKey is string keyText)
                {
                    _keys.TryRemove(keyText, out _);
                }
            });

            memoryCache.Set(key, value, options);
            _keys.TryAdd(key, 0);

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            memoryCache.Remove(key);
            _keys.TryRemove(key, out _);

            return Task.CompletedTask;
        }

        public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
        {
            foreach (var key in _keys.Keys.Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
            {
                memoryCache.Remove(key);
                _keys.TryRemove(key, out _);
            }

            return Task.CompletedTask;
        }
    }
}
