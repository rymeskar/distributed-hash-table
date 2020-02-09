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

        public async Task<DistributedResult> GetAsync(string key)
        {
            (var node, var actualKey) = GetAddressableKey(key);

            Result result;
            if (!CanHandle(node))
            {
                try
                {
                    var retVal = await _remoteNodeHashTable.GetAsync(key);
                    _logger.LogInformation($"{key} retrieved from {retVal.Node}");
                    return retVal;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{key} could not be retrieved from remote");
                }
            }

            try
            {
                result = await _inMemoryHashTable.GetAsync(actualKey);
            }
            catch (KeyNotFoundException)
            {
                result = await _backupHashTable.GetAsync(actualKey);
                await _inMemoryHashTable.StoreAsync(actualKey, result.Value);
            }

            _logger.LogInformation($"{key} retrieved from {result.MemorySource}.");

            return new DistributedResult(result, node);
        }

        public async Task<NodeIdentifier> RemoveAsync(string key)
        {
            (var node, var actualKey) = GetAddressableKey(key);

            if (!CanHandle(node))
            {
                try
                { 
                    var handlingNode = await _remoteNodeHashTable.RemoveAsync(key);
                    return handlingNode;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{key} could not be removed from remote");
                }
            }

            var backupTask = _backupHashTable.RemoveAsync(actualKey);
            await _inMemoryHashTable.RemoveAsync(actualKey);
            await backupTask;

            return node;
        }

        public async Task<NodeIdentifier> StoreAsync(string key, string value)
        {
            (var node, var actualKey) = GetAddressableKey(key);

            if (!CanHandle(node))
            {
                try
                { 
                    var handlingNode = await _remoteNodeHashTable.StoreAsync(key, value);
                    return handlingNode;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{key} could not be stored on remote");
                }

            }

            // TODO: should also ensure atomicity for backup/inMemory!
            var backupTask = _backupHashTable.StoreAsync(actualKey, value);
            await _inMemoryHashTable.StoreAsync(actualKey, value);
            await backupTask;

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
