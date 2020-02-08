using Library.KeySpace;
using Library.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class DistributedHashTable : IDistributedHashTable
    {
        private readonly IKeySpaceHash _hasher;
        private readonly IKeySpaceManager _manager;
        private readonly IInMemoryHashTable _inMemoryHashTable;
        private readonly IBackupHashTable _backupHashTable;
        private readonly IRemoteNodeHashTable _remoteNodeHashTable;
        private readonly ILogger<DistributedHashTable> _logger;
        private readonly NodeIdentifier _currentNode;

        public DistributedHashTable(IKeySpaceHash hasher,
            IKeySpaceManager manager,
            IInMemoryHashTable inMemoryHashTable, 
            IBackupHashTable backupHashTable,
            IRemoteNodeHashTable remoteNodeHashTable,
            INodeIdentifierFactory nodeIdentifierFactory,
            ILogger<DistributedHashTable> logger)
        {
            _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _inMemoryHashTable = inMemoryHashTable ?? throw new ArgumentNullException(nameof(inMemoryHashTable));
            _backupHashTable = backupHashTable ?? throw new ArgumentNullException(nameof(backupHashTable));
            _remoteNodeHashTable = remoteNodeHashTable;
            _logger = logger;
            _currentNode = nodeIdentifierFactory.Create();
        }

        // TODO: fix when remote is out -- force update
        // TODO: cache expiration
        // TODO: liveness cache
        // TODO: DNS cache prepopulation
        public async Task<DistributedResult> GetAsync(string key)
        {
            (var node, var actualKey) = GetAddressableKey(key);

            if (CanHandle(node))
            {
                Result retVal;
                try
                {
                    retVal = await _inMemoryHashTable.GetAsync(actualKey);
                }
                catch (KeyNotFoundException)
                {
                    retVal = await _backupHashTable.GetAsync(actualKey);
                    await _inMemoryHashTable.StoreAsync(actualKey, retVal.Value);
                }

                _logger.LogInformation($"{key} retrieved from {retVal.MemorySource}.");

                return new DistributedResult(retVal, node);
            }
            else
            {
                var retVal = await _remoteNodeHashTable.GetAsync(key);
                _logger.LogInformation($"{key} retrieved from {retVal.Node}");
                return retVal;
            }
        }

        public async Task<NodeIdentifier> RemoveAsync(string key)
        {
            (var node, var actualKey) = GetAddressableKey(key);
            if (CanHandle(node))
            {
                // TODO: need to ensure atomicity of the below two requests.
                await _backupHashTable.RemoveAsync(actualKey);
                await _inMemoryHashTable.RemoveAsync(actualKey);
            }
            else
            {
                await _remoteNodeHashTable.RemoveAsync(key);
            }

            return node;
        }

        public async Task<NodeIdentifier> StoreAsync(string key, string value)
        {
            (var node, var actualKey) = GetAddressableKey(key);

            if (CanHandle(node))
            {
                // TODO: need to ensure atomicity of the below two requests.
                _backupHashTable.StoreAsync(actualKey, value);
                await _inMemoryHashTable.StoreAsync(actualKey, value);
            }
            else
            {
                await _remoteNodeHashTable.StoreAsync(key, value);
            }

            return node;
        }

        private bool CanHandle(NodeIdentifier node)
        {
            return node.Equals(_currentNode);
        }
        private (NodeIdentifier, Key) GetAddressableKey(string key)
        {
            var actualKey = _hasher.Hash(key);
            var node = _manager.GetHandlingNode(actualKey);

            return (node, actualKey);
        }
    }
}
