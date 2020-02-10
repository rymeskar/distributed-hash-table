using DistributedHashTable.DHT;
using Grpc.Net.Client;
using Library;
using Library.Broker;
using Library.KeySpace;
using Library.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DistributedHashTable
{
    public class KubernetesNodeHashTable : IRemoteNodeHashTable
    {
        private readonly IKeySpaceHash _hasher;
        private readonly IKeySpaceManager _manager;
        private readonly ILogger<KubernetesNodeHashTable> _logger;
        private readonly IAddressTranslation _addressTranslation;
        private readonly IClusterBrokerService _clusterBrokerService;
        private HttpClient _httpClient;

        public KubernetesNodeHashTable(IKeySpaceHash hasher, IKeySpaceManager manager, IAddressTranslation addressTranslation, ILogger<KubernetesNodeHashTable> logger, IClusterBrokerService brokerService)
        {
            _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _addressTranslation = addressTranslation ?? throw new ArgumentNullException(nameof(addressTranslation));
            _clusterBrokerService = brokerService;

            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            _httpClient = new HttpClient(httpClientHandler);
        }

        public async Task<DistributedResult> GetAsync(string key)
        {
            NodeIdentifier node = null;
            Address address = null;
            try
            {
                (node, address) = await GetAddress(key);
                using var channel = GrpcChannel.ForAddress($"https://{address.NetworkAddress}:5001", new GrpcChannelOptions { HttpClient = _httpClient });
                var dhtClient = new HashTable.HashTableClient(channel);

                var response = await dhtClient.GetAsync(DhtClientHelpers.CreateGetRequest(key));

                return new DistributedResult(GetResult(response), node);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Did not receive response from node {node} and {address}.");
                await _clusterBrokerService.KeepUpToDate();
                throw;
            }
        }

        public Task<NodeIdentifier> RemoveAsync(string key)
        {
            throw new NotImplementedException("TODO: introduce Remove");
        }

        public async Task<NodeIdentifier> StoreAsync(string key, string value)
        {
            NodeIdentifier node = null;
            Address address = null;
            try
            {
                (node, address) = await GetAddress(key);
                using var channel = GrpcChannel.ForAddress($"https://{address.NetworkAddress}:5001", new GrpcChannelOptions { HttpClient = _httpClient });
                var dhtClient = new HashTable.HashTableClient(channel);

                var response = await dhtClient.StoreAsync(DhtClientHelpers.CreateStoreRequest(key, value));

                return node;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Did not receive response from node {node} and {address}.");
                await _clusterBrokerService.KeepUpToDate();
                throw;
            }
        }

        private async Task<(NodeIdentifier, Address)> GetAddress(string key)
        {
            var actualKey = _hasher.Hash(key);
            var node = _manager.GetHandlingNode(actualKey);
            return (node, await _addressTranslation.TranslateAsync(node.Name));
        }


        public static Result GetResult(ValueResponse response)
        {
            // TODO: make sure info is propagated properly
            return new Result(response.Value, "RemoteNode");
        }
    }
}
