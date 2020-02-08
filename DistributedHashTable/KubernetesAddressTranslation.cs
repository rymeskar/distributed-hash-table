using k8s;
using Library;
using Library.Broker;
using Microsoft.Extensions.Logging;
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
        // TODO: add caching!
        private readonly IKubernetes _kubernetesClient;
        private readonly IDictionary<string, Address> _logicalToAddress;
        private readonly ILogger<KubernetesAddressTranslation> _logger;

        public KubernetesAddressTranslation(IKubernetes kubernetesClient, ILogger<KubernetesAddressTranslation> logger)
        {
            _kubernetesClient = kubernetesClient ?? throw new ArgumentNullException(nameof(kubernetesClient));
            _logger = logger;
            _logicalToAddress = new ConcurrentDictionary<string, Address>();
        }

        public async Task<Address> TranslateAsync(string podName)
        {
            if (_logicalToAddress.TryGetValue(podName, out var address))
            {
                return address;
            }
            var list = await _kubernetesClient.ListNamespacedPodAsync("dht");

            foreach (var item in list.Items)
            {
                _logger.LogDebug($"Name: {item.Metadata.Name} Alias: {item.Status.PodIP}");

                address = new Address(item.Status.PodIP);

                _logicalToAddress[podName] = address;
            }

            return _logicalToAddress[podName];
        }
    }
}
