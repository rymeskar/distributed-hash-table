using Library.KeySpace;
using System;
using System.Threading.Tasks;

namespace Library.Broker
{
    public class ClusterBrokerService : IClusterBrokerService
    {
        private readonly IClusterManager _clusterManager;
        private readonly IAddressTranslation _addressTranslation;
        private readonly IKeySpaceManager _keySpaceManager;

        public ClusterBrokerService(IClusterManager clusterManager, IAddressTranslation addressTranslation, IKeySpaceManager keySpaceManager)
        {
            _clusterManager = clusterManager ?? throw new ArgumentNullException(nameof(clusterManager));
            _addressTranslation = addressTranslation ?? throw new ArgumentNullException(nameof(addressTranslation));
            _keySpaceManager = keySpaceManager ?? throw new ArgumentNullException(nameof(keySpaceManager));
        }

        public async Task<bool> TryAddNode(NodeIdentifier node)
        {
            if (!_clusterManager.ContainsNode(node))
            {
                var address = await _addressTranslation.TranslateAsync(node.Name);
                if (await _clusterManager.AddNodeAsync(node, address))
                {
                    _keySpaceManager.AddNode(node);
                    return true;
                }
            }

            return false;
        }

        public async Task KeepUpToDate()
        {
            var toRemove = await _clusterManager.FindDeadNodesAsync();
            _clusterManager.RemoveNodes(toRemove);
            _keySpaceManager.RemoveNodes(toRemove);
        }
    }
}
