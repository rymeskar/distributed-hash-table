using DistributedHashTable.Broker;
using Grpc.Core;
using Library;
using Library.Broker;
using Library.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DistributedHashTable.Services
{
    public class BrokerService : Broker.Broker.BrokerBase
    {
        private readonly ILogger<BrokerService> _logger;
        private readonly NodeIdentifier _nodeIdentifier;
        private readonly IClusterBrokerService _clusterBrokerService;

        public BrokerService(ILogger<BrokerService> logger, INodeIdentifierFactory nodeIdentifierFactory, IClusterBrokerService clusterBrokerService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _nodeIdentifier = nodeIdentifierFactory.Create();
            _clusterBrokerService = clusterBrokerService;
        }

        public override async Task<SystemInfo> Ping(SystemInfo request, ServerCallContext context)
        {
            var caller = GetNode(request);
            if (await _clusterBrokerService.TryAddNode(caller))
            {
                _logger.LogInformation($"Ping added a new node: {request.Identifier}");
            }
            return GetSystemInfo(_nodeIdentifier);
        }

        private SystemInfo GetSystemInfo(NodeIdentifier node)
        {
            return new SystemInfo
            {
                Identifier = node.Name,
                KeyId = node.Key.Base64EncodedKey
            };
        }

        private NodeIdentifier GetNode(SystemInfo info)
        {
            return new NodeIdentifier(info.Identifier, Key.Create(info.KeyId));
        }
    }
}
