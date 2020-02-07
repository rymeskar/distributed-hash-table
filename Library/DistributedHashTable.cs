using Library.KeySpace;
using Library.Model;
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
        private readonly IRemoteHashTable _remoteHashTable;

        public DistributedHashTable(IKeySpaceHash hasher, IKeySpaceManager manager, IInMemoryHashTable inMemoryHashTable, IRemoteHashTable remoteHashTable)
        {
            _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _inMemoryHashTable = inMemoryHashTable ?? throw new ArgumentNullException(nameof(inMemoryHashTable));
            _remoteHashTable = remoteHashTable ?? throw new ArgumentNullException(nameof(remoteHashTable));
        }

        public async Task<Result> GetAsync(string key)
        {
            var actualKey = GetKey(key);
            try
            {
                return await _inMemoryHashTable.GetAsync(actualKey);
            }
            catch (KeyNotFoundException)
            {
                return await _remoteHashTable.GetAsync(actualKey);
            }
        }

        public async Task RemoveAsync(string key)
        {
            var actualKey = GetKey(key);
            _remoteHashTable.RemoveAsync(actualKey);
            await _inMemoryHashTable.RemoveAsync(actualKey);
        }

        public async Task StoreAsync(string key, string value)
        {
            var actualKey = GetKey(key);
            _remoteHashTable.StoreAsync(actualKey, value);
            await _inMemoryHashTable.StoreAsync(actualKey, value);
        }

        private Key GetKey(string key)
        {
            var actualKey = _hasher.Hash(key);
            if (!_manager.CanHandle(actualKey))
            {
                throw new InvalidOperationException("Cannot Handle");
            }

            return actualKey;
        }
    }
}
