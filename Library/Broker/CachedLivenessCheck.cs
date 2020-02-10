using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Library.Broker
{
    public class CachedLivenessCheck : ICachedLivenessCheck
    {
        private const string CachePrefix = nameof(CachedLivenessCheck);
        private readonly ILivenessCheck _livenessCheck;
        private readonly IMemoryCache _memoryCache;
        private readonly IOptions<DHTOptions> _options;

        public CachedLivenessCheck(IMemoryCache memoryCache, ILivenessCheck livenessCheck, IOptions<DHTOptions> options)
        {
            _livenessCheck = livenessCheck ?? throw new ArgumentNullException(nameof(livenessCheck));
            _memoryCache = memoryCache;
            _options = options;
        }

        public async Task<bool> IsAliveAsync(Address address)
        {
            if (_memoryCache.TryGetValue<bool>(GetKey(address), out var result))
            {
                return result;
            }

            var retVal = await _livenessCheck.IsAliveAsync(address);

            if (retVal == true)
            {
                _memoryCache.Set(GetKey(address), retVal, _options.Value.LivenessCaching);
            }

            return retVal;
        }

        private static string GetKey(Address address) => $"{CachePrefix}_{address.NetworkAddress}";

    }
}
