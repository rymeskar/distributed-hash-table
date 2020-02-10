using Grpc.Net.Client;
using Library;
using Library.Broker;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DistributedHashTable
{
    public class KubernetesLivenessCheck : ILivenessCheck
    {
        private ILogger<KubernetesLivenessCheck> _logger;
        private NodeIdentifier _identifier;
        private HttpClient _httpClient;

        public KubernetesLivenessCheck(ILogger<KubernetesLivenessCheck> logger, INodeIdentifierFactory factory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _identifier = factory.Create();

            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            _httpClient = new HttpClient(httpClientHandler);
        }

        public async Task<bool> IsAliveAsync(Address address)
        {
            if (address.IsLocal)
            {
                return true;
            }

            try
            {
                using var pingChannel = GrpcChannel.ForAddress($"https://{address.NetworkAddress}:5001", new GrpcChannelOptions { HttpClient = _httpClient });
                var pingClinet = new Greeter.GreeterClient(pingChannel);
                var pingResponse = await pingClinet.SayHelloAsync(new HelloRequest());
                _logger.LogDebug($"I am {_identifier.Name} and got ping back with message\"{pingResponse.Message}\"");

                return true;
            }
            catch
            {
                _logger.LogWarning($"I am {_identifier.Name} and did not get a ping back from {address.NetworkAddress}");

                return false;
            }
        }
    }
}
