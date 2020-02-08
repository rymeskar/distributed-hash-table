using DistributedHashTable.Broker;
using Grpc.Core;
using Grpc.Net.Client;
using k8s;
using Library;
using Library.Broker;
using Library.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public ConsistencyService(ILogger<ConsistencyService> logger, INodeIdentifierFactory factory, IClusterBrokerService clusterBrokerService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _identifier = factory.Create();
            _clusterBrokerService = clusterBrokerService;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // TODO: Init somewhere else!
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var httpClient = new HttpClient(httpClientHandler);
            using var channel = GrpcChannel.ForAddress($"https://dht:5001", new GrpcChannelOptions { HttpClient = httpClient  });
            var brokerClient = new Broker.Broker.BrokerClient(channel);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _clusterBrokerService.KeepUpToDate();

                    var response = await brokerClient.PingAsync(GetSystemInfo());
                    _logger.LogInformation($"I am {_identifier.Name} and got response by {response.Identifier}");
                    
                    if (await _clusterBrokerService.TryAddNode(GetNode(response)))
                    {
                        _logger.LogInformation($"Consistency added a new node: {response.Identifier}");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Did not receive pong.");
                }

                await Task.Delay(TimeSpan.FromSeconds(10));
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
