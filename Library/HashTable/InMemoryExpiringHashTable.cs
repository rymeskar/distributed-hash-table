using Library.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Library.HashTable
{
    public class InMemoryExpiringHashTable : IInMemoryHashTable
    {
        private const string CachePrefix = nameof(InMemoryExpiringHashTable);
        private readonly IMemoryCache _memoryCache;
        private readonly IOptions<DHTOptions> _options;

        public InMemoryExpiringHashTable(IMemoryCache memoryCache, IOptions<DHTOptions> options)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Task StoreAsync(Key key, string value)
        {
            _memoryCache.Set(GetKey(key), value, _options.Value.InMemoryCaching);
            return Task.CompletedTask;
        }

        public Task<Result> GetAsync(Key key)
        {
            var value = _memoryCache.GetOrThrow<string>(GetKey(key));

            return Task.FromResult(new Result(value, "InMemory"));
        }

        public Task RemoveAsync(Key key)
        {
            _memoryCache.Remove(GetKey(key));
            return Task.CompletedTask;
        }

        private static string GetKey(Key key) => $"{CachePrefix}_{key.Base64EncodedKey}";
    }
}
