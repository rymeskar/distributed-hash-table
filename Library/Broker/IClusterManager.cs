using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library.Broker
{
    public interface IClusterManager
    {
        public NodeIdentifier LocalNode { get; }
        public Task<bool> AddNodeAsync(NodeIdentifier node, Address address);
        public void RemoveNodes(IList<NodeIdentifier> nodes);
        public bool ContainsNode(NodeIdentifier node);
        public Task<IList<NodeIdentifier>> FindDeadNodesAsync();
    }
}
