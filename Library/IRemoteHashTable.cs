using Library.KeySpace;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public interface IRemoteHashTable : IHashTable { }

    public class RemoteHashTable : IRemoteHashTable
    {
        public Task<Result> GetAsync(Key key)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(Key key)
        {
            throw new NotImplementedException();
        }

        public Task StoreAsync(Key key, string value)
        {
            throw new NotImplementedException();
        }
    }
}
