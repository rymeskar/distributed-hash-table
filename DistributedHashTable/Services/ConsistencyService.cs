using DistributedHashTable.Broker;
using Grpc.Core;
using Grpc.Net.Client;
using k8s;
using Library;
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
        private IDnsResolution _dnsResolution;

        public ConsistencyService(ILogger<ConsistencyService> logger, INodeIdentifierFactory factory, IDnsResolution dnsResolution)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _identifier = factory.Create();
            _dnsResolution = dnsResolution;
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
                    var response = await brokerClient.PingAsync(GetSystemInfo());
                    _logger.LogInformation($"I am {_identifier.Name} and got response by {response.Identifier}");
                    
                    var ip = await _dnsResolution.ResolveAsync(response.Identifier);


                    using var pingChannel = GrpcChannel.ForAddress($"https://{ip}:5001", new GrpcChannelOptions { HttpClient = httpClient });
                    var pingClinet = new Greeter.GreeterClient(pingChannel);
                    var pingResponse = await pingClinet.SayHelloAsync(new HelloRequest());
                    _logger.LogInformation($"I am {_identifier.Name} and got ping back with message\"{pingResponse.Message}\"");
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
    }
}
