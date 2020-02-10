using Library.Broker;
using Library.KeySpace;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Library
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDistributedHashTable(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDistributedHashTable, DistributedHashTable>();
            serviceCollection.AddSingleton<IKeySpaceHash, KeySpaceHash>();
            serviceCollection.AddSingleton<IKeySpaceManager, LocalKeySpaceManager>();
            serviceCollection.AddSingleton<IBackupHashTable, RemoteHashTable>();
            serviceCollection.AddSingleton<IInMemoryHashTable, InMemoryPermanentHashTable>();
            serviceCollection.AddSingleton<INodeIdentifierFactory, NodeIdentifierFactory>();

            serviceCollection.AddSingleton<IKeySpaceManager, DistributedKeySpaceManager>();
            serviceCollection.AddSingleton<IClusterManager, ClusterManager>();
            serviceCollection.AddSingleton<IClusterBrokerService, ClusterBrokerService>();

            serviceCollection.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()));
            return serviceCollection;
        }
    }
}
