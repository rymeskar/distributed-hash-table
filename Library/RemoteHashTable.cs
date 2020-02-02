using Library.KeySpace;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public interface IRemoteHashTable : IHashTable { }


    public class RemoteHashTable : IRemoteHashTable
    {
        private class CosmosDbItem
        {
            public string id { get;set; }
            public string key { get; set; }
            public string Value { get; set; }
        }

        private readonly Container _container;

        public RemoteHashTable()
        {
            // TODO: better
            var cosmosClient = new CosmosClient("AccountEndpoint=https://hash-table-sql.documents.azure.com:443/;AccountKey=4Ul5DBQhAsr48BceNFNfrxzUYcwMLUmMumbXDSbllIejaeKKB6EJ2dY1c1STv1paOBUwPYBBiB9sNU7HPieUuA==;");
            _container = cosmosClient.GetContainer("hash-table", "hash-table");
        }

        public async Task<Result> GetAsync(Key key)
        {
            try
            {
                var item = await _container.ReadItemAsync<CosmosDbItem>(key.Base64EncodedKey, new PartitionKey(key.Base64EncodedKey));
                return new Result(item.Resource.Value, "CosmosDb");
            } catch (Exception e)
            {
                throw new KeyNotFoundException("Message not found in CosmosDb", e);
            }

        }

        public async Task RemoveAsync(Key key)
        {
            await _container.DeleteItemAsync<CosmosDbItem>(key.Base64EncodedKey, new PartitionKey(key.Base64EncodedKey));
        }

        public async Task StoreAsync(Key key, string value)
        {
            var item = new CosmosDbItem()
            {
                key = key.Base64EncodedKey,
                Value = value,
                id = key.Base64EncodedKey
            };

            await _container.CreateItemAsync(item);
        }
    }
}
