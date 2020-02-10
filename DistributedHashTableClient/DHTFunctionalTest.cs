using DistributedHashTable;
using DistributedHashTable.DHT;
using Library;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using static DistributedHashTable.DHT.HashTable;

namespace DistributedHashTableClient
{
    public class DHTFunctionalTest
    {
        private readonly string Key = "123";
        private readonly string Value = "1";

        private readonly HashTableClient _client;
        private readonly ILogger<DHTFunctionalTest> _logger;
        private readonly KubernetesState _kubernetesState;

        public DHTFunctionalTest(HashTableClient client, ILogger<DHTFunctionalTest> logger, KubernetesState kubernetesState)
        {
            this._client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger;
            _kubernetesState = kubernetesState;
        }

        public async Task ThrowCases()
        {
            try
            {
                await _client.GetAsync(DhtClientHelpers.CreateGetRequest(Key));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while Getting Key");
            }
        }
        public Task StoreGet() => StoreGet(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        private async Task StoreGet(string key, string value)
        {
            await _kubernetesState.RefreshAsync();
            await _client.StoreAsync(DhtClientHelpers.CreateStoreRequest(key, value));
            var response = await _client.GetAsync(DhtClientHelpers.CreateGetRequest(key));
            var supposed = _kubernetesState.Manager.GetHandlingNode(_kubernetesState.Hasher.Hash(key));
            Log(response, value, supposed);
        }


        public async Task RandomTest()
        {
            var tasks = Enumerable.Range(0, 100).Select(t => StoreGet()).ToList();
            await Task.WhenAll(tasks);
            var allCompleted = tasks.All(t => t.IsCompleted);
        }

        public void Log(ValueResponse response, string originalValue, NodeIdentifier supposed)
        {

            if (originalValue != response.Value)
            {
                _logger.LogError($"Response does not match! {response.Value}; original: {originalValue}");
            }

            if (response.Info.Identifier != supposed.Name)
            {
                _logger.LogWarning($"Respone: {response.Value}, From: {response.Info.Identifier}, Supposed: {supposed.Name}");
            }

            _logger.LogInformation($"Respone: {response.Value}, From: {response.Info.Identifier}");
        }
    }
}
