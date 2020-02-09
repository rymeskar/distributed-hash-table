using k8s;
using Library;
using Library.Broker;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DistributedHashTable
{
    public class KubernetesAddressTranslation : IAddressTranslation
    {
        private const string CachePrefix = nameof(KubernetesAddressTranslation);

        private readonly IKubernetes _kubernetesClient;
        private readonly ILogger<KubernetesAddressTranslation> _logger;
        private readonly IOptions<DHTOptions> _options;
        private readonly IMemoryCache _memoryCache;

        public KubernetesAddressTranslation(IKubernetes kubernetesClient, ILogger<KubernetesAddressTranslation> logger, IOptions<DHTOptions> options, IMemoryCache memoryCache)
        {
            _kubernetesClient = kubernetesClient ?? throw new ArgumentNullException(nameof(kubernetesClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async Task<Address> TranslateAsync(string podName)
        {
            var key = GetKey(podName);
            if (_memoryCache.TryGetValue<Address>(key, out var address))
            {
                return address;
            }
            var list = await _kubernetesClient.ListNamespacedPodAsync("dht");

            foreach (var item in list.Items)
            {
                _logger.LogDebug($"Name: {item.Metadata.Name} Alias: {item.Status.PodIP}");

                address = new Address(item.Status.PodIP);

                _memoryCache.Set(key, address, _options.Value.TranslationCaching);
            }

            return _memoryCache.Get<Address>(key);
        }

        private static string GetKey(string logicalName) => $"{CachePrefix}_{logicalName}";
    }
}
