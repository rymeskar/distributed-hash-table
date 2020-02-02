using DistributedHashTable;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using static DistributedHashTable.HashTable;
using Microsoft.Extensions.Logging.Console;

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

            using var channel = GrpcChannel.ForAddress($"https://localhost:{args[0]}", new GrpcChannelOptions { HttpClient = httpClient });
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(new HelloRequest { Name = "Karel" });
            Console.WriteLine("Greeting: " + reply.Message);

            var dhtClient = new HashTable.HashTableClient(channel); 
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddConsole());
            serviceCollection.AddSingleton(dhtClient);
            serviceCollection.AddSingleton<DHTFunctionalTest>();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var funcTest = serviceProvider.GetService<DHTFunctionalTest>();
            
            await funcTest.ThrowCases();
            await funcTest.StoreGet();
            await funcTest.RandomTest();

            serviceProvider.Dispose();
        }
    }
}
