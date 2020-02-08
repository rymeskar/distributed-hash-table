using Library.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public interface IRemoteNodeHashTable : IDistributedHashTable
    {
    }

    public class NullRemoteNodeHashTable : IRemoteNodeHashTable
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
