using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Hjerpbakk.DIPSBot.Extensions {
    public static class MemoryCacheExtensions {
        static readonly MemoryCacheEntryOptions cacheEntryOptions;

        static MemoryCacheExtensions() {
            cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(1));
        }

        public static async Task<T> GetOrSet<T>(this IMemoryCache memoryCache, object key, Func<Task<T>> create) {
            if (!memoryCache.TryGetValue(key, out T result)) {
                result = await create();
                memoryCache.Set(key, result, cacheEntryOptions);
            }

            return result;
        }
    }
}
