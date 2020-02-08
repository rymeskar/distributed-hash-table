using Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace Library.Broker
{
    public class ClusterManager : IClusterManager
    {
        private readonly ILivenessCheck _livenessCheck;
        private readonly IDictionary<NodeIdentifier, Address> _cluster;
        private readonly ILogger<ClusterManager> _logger;

        public ClusterManager(ILivenessCheck livenessCheck, ILogger<ClusterManager> logger, INodeIdentifierFactory nodeIdentifierFactory)
        {
            _livenessCheck = livenessCheck ?? throw new ArgumentNullException(nameof(livenessCheck));
            _cluster = new Dictionary<NodeIdentifier, Address>();
            _logger = logger;
            LocalNode = nodeIdentifierFactory.Create();
        }

        public NodeIdentifier LocalNode { get; }

        public async Task<bool> AddNodeAsync(NodeIdentifier node, Address address)
        {
            if (!await _livenessCheck.IsAliveAsync(address))
            {
                return false;
            }
            else
            {
                _cluster.Add(node, address);
                _logger.LogInformation($"Added node {node} to the cluster");
                return true;
            }
        }

        public bool ContainsNode(NodeIdentifier node)
        {

            return node.Equals(LocalNode) || _cluster.ContainsKey(node);
        }

        public async Task<IList<NodeIdentifier>> FindDeadNodesAsync()
        {
            var retVal = new List<NodeIdentifier>();
            var results = _cluster.Select(n => (n.Key, _livenessCheck.IsAliveAsync(n.Value)));
            await Task.WhenAll(results.Select(a => a.Item2));

            return results.Where(n => n.Item2.Result == false).Select(n => n.Item1).ToList();
        }

        public void RemoveNodes(IList<NodeIdentifier> nodes)
        {
            foreach (var node in nodes)
            {
                _cluster.Remove(node);
                _logger.LogInformation($"Removed node {node} from the cluster");
            }
        }
    }
}
