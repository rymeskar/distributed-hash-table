using System;
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

        public DHTService(IDistributedHashTable hashTable, ILogger<DHTService> logger)
        {
            _hashTable = hashTable ?? throw new ArgumentNullException(nameof(hashTable));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async override Task<ValueResponse> Get(GetRequest request, ServerCallContext context)
        {
            var value = await _hashTable.GetAsync(request.Key);

            return new ValueResponse()
            {
                Info = GetSystemInfo(value.Node),
                Value = value.Result.Value
            };
        }

        public async override Task<SystemInfo> Store(StoreRequest request, ServerCallContext context)
        {
            var node = await _hashTable.StoreAsync(request.Key, request.Value);
            return GetSystemInfo(node);
        }

        private SystemInfo GetSystemInfo(NodeIdentifier node)
        {
            return new SystemInfo
            {
                Identifier = node.Name,
                KeyId = node.Key.Base64EncodedKey
            };
        }
    }
}
