using DistributedHashTable.DHT;

namespace DistributedHashTable
{
    public static class DhtClientHelpers
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
