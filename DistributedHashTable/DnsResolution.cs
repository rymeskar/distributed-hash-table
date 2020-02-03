using k8s;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DistributedHashTable
{
    public class DnsResolution : IDnsResolution
    {
        // TODO: add caching!
        private readonly IKubernetes _kubernetesClient;
        private readonly ILogger<DnsResolution> _logger;

        public DnsResolution(IKubernetes kubernetesClient, ILogger<DnsResolution> logger)
        {
            _kubernetesClient = kubernetesClient ?? throw new ArgumentNullException(nameof(kubernetesClient));
            _logger = logger;
        }

        public async Task<string> ResolveAsync(string podName)
        {
            var list = await _kubernetesClient.ListNamespacedPodAsync("dht");

            foreach (var item in list.Items)
            {
                _logger.LogDebug($"Name: {item.Metadata.Name} Alias: {item.Status.PodIP}");

                if (item.Metadata.Name == podName)
                {
                    return item.Status.PodIP;
                }
            }

            throw new Exception("Pod not found");
        }
    }
}
