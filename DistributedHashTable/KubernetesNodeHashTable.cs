using Library;
using Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedHashTable
{
    public class KubernetesNodeHashTable : IRemoteNodeHashTable
    {
        public Task<DistributedResult> GetAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<NodeIdentifier> RemoveAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<NodeIdentifier> StoreAsync(string key, string value)
        {
            throw new NotImplementedException();
        }
    }
}
