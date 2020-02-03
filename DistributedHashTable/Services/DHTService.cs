using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DistributedHashTable.DHT;
using Grpc.Core;
using Library;
using Microsoft.Extensions.Logging;

namespace DistributedHashTable
{
    public class DHTService : HashTable.HashTableBase
    {
        private readonly IDistributedHashTable _hashTable;
        private readonly ILogger<DHTService> _logger;
        private readonly NodeIdentifier _identifier;

        public DHTService(IDistributedHashTable hashTable, ILogger<DHTService> logger, INodeIdentifierFactory factory)
        {
            _hashTable = hashTable ?? throw new ArgumentNullException(nameof(hashTable));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _identifier = factory.Create();
        }

        public async override Task<ValueResponse> Get(GetRequest request, ServerCallContext context)
        {
            var value = await _hashTable.GetAsync(request.Key);

            return new ValueResponse()
            {
                Info = GetSystemInfo(),
                Value = value.Value
            };
        }

        public async override Task<SystemInfo> Store(StoreRequest request, ServerCallContext context)
        {
            await _hashTable.StoreAsync(request.Key, request.Value);
            return GetSystemInfo();
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
