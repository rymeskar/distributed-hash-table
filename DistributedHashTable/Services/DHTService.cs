using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                Info = GetSystemInfo(context),
                Value = value.Value
            };
        }

        public async override Task<SystemInfo> Store(StoreRequest request, ServerCallContext context)
        {
            await _hashTable.StoreAsync(request.Key, request.Value);
            return GetSystemInfo(context);
        }

        private SystemInfo GetSystemInfo(ServerCallContext context)
        {
            return new SystemInfo
            {
                Identifier = context.Host,
                KeyId = context.Host,
            };
        }
    }
}
