using Library.Broker;
using Library.KeySpace;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

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
            serviceCollection.AddSingleton<IInMemoryHashTable, InMemoryHashTable>();
            serviceCollection.AddSingleton<INodeIdentifierFactory, NodeIdentifierFactory>();

            serviceCollection.AddSingleton<IKeySpaceManager, DistributedKeySpaceManager>();
            serviceCollection.AddSingleton<IClusterManager, ClusterManager>();
            serviceCollection.AddSingleton<IClusterBrokerService, ClusterBrokerService>();
            return serviceCollection;
        }
    }
}
