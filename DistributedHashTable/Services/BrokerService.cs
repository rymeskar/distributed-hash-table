using DistributedHashTable.Broker;
using Grpc.Core;
using Library;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedHashTable.Services
{
    public class BrokerService : Broker.Broker.BrokerBase
    {
        private readonly ILogger<BrokerService> _logger;
        private readonly NodeIdentifier _identifier;

        public BrokerService(IDistributedHashTable hashTable, ILogger<BrokerService> logger, INodeIdentifierFactory factory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _identifier = factory.Create();
        }

        public override Task<SystemInfo> Ping(SystemInfo request, ServerCallContext context)
        {
            return Task.FromResult(GetSystemInfo());
        }

        private SystemInfo GetSystemInfo()
        {
            return new SystemInfo
            {
                Identifier = _identifier.Name,
                KeyId = _identifier.Key.Base64EncodedKey
            };
        }
    }
}
