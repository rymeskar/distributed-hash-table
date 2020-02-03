using DistributedHashTable;
using DistributedHashTable.DHT;
using System;
using System.Collections.Generic;
using System.Text;

namespace DistributedHashTableClient
{
    public static class GRPCHelpers
    {
        public static GetRequest CreateGetRequest(string key)
        {
            return new GetRequest
            {
                Key = key
            };
        }

        public static StoreRequest CreateStoreRequest(string key, string value)
        {
            return new StoreRequest
            {
                Key = key,
                Value = value
            };
        }
    }
}
