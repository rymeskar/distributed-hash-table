using Library;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using k8s;
using Library.KeySpace;

namespace DistributedHashTableClient
{
    public class KubernetesState
    {
        private readonly IKubernetes _kubernetesClient;
        public IKeySpaceHash Hasher { get; }
        public IKeySpaceManager Manager { get; private set; }

        public KubernetesState(IKubernetes kubernetesClient, IKeySpaceHash hasher)
        {
            _kubernetesClient = kubernetesClient ?? throw new ArgumentNullException(nameof(kubernetesClient));
            Hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        }

        public async Task RefreshAsync()
        {
            var list = await _kubernetesClient.ListNamespacedPodAsync("dht");

            var nodes = new List<NodeIdentifier>();
            foreach (var item in list.Items)
            {
                var podName = item.Metadata.Name;
                if (!podName.StartsWith("dht-"))
                {
                    break;
                }

                var node = new NodeIdentifier(podName, Hasher.Hash(podName));
                nodes.Add(node);
            }

            Manager = DistributedKeySpaceManager.Create(nodes);
        }
    }
}
