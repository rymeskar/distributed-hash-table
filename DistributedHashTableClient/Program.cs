using DistributedHashTable;
using Grpc.Net.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DistributedHashTableClient
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var httpClientHandler = new HttpClientHandler();
            // Return `true` to allow certificates that are untrusted/invalid
            httpClientHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var httpClient = new HttpClient(httpClientHandler);

            using var channel = GrpcChannel.ForAddress("https://localhost:32794", new GrpcChannelOptions { HttpClient = httpClient });
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(
                              new HelloRequest { Name = "Karel" });
            Console.WriteLine("Greeting: " + reply.Message);
        }
    }
}
