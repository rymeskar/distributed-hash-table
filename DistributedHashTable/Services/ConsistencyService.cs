using DistributedHashTable.Broker;
using Grpc.Net.Client;
using Library;
using Library.Broker;
using Library.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedHashTable.Services
{
    public class ConsistencyService : BackgroundService
    {
        private ILogger<ConsistencyService> _logger;
        private NodeIdentifier _identifier;
        private IClusterBrokerService _clusterBrokerService;
        private IOptions<DHTOptions> _options;
        private HttpClient _httpClient;

        public ConsistencyService(ILogger<ConsistencyService> logger, INodeIdentifierFactory factory, IClusterBrokerService clusterBrokerService, IOptions<DHTOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _identifier = factory.Create();
            _clusterBrokerService = clusterBrokerService;
            _options = options;

            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            _httpClient = new HttpClient(httpClientHandler);
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var channel = GrpcChannel.ForAddress($"https://dht:5001", new GrpcChannelOptions { HttpClient = _httpClient });
            var brokerClient = new Broker.Broker.BrokerClient(channel);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    ////await _clusterBrokerService.TryAddNode(new NodeIdentifier("a", new KeySpaceHash().Hash("a")));
                    await _clusterBrokerService.KeepUpToDate();

                    var response = await brokerClient.PingAsync(GetSystemInfo());
                    
                    if (await _clusterBrokerService.TryAddNode(GetNode(response)))
                    {
                        _logger.LogInformation($"Consistency added a new node: {response.Identifier}");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Did not receive pong.");
                }

                await Task.Delay(_options.Value.ConsistencyPeriod);
            }
        }

        private SystemInfo GetSystemInfo()
        {
            return new SystemInfo
            {
                Identifier = _identifier.Name,
                KeyId = _identifier.Key.Base64EncodedKey
            };
        }

        private NodeIdentifier GetNode(SystemInfo info)
        {
            return new NodeIdentifier(info.Identifier, Key.Create(info.KeyId));
        }
    }
}
