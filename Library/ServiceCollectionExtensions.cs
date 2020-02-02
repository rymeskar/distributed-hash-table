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
            serviceCollection.AddSingleton<IKeySpaceManager, KeySpaceManager>();
            serviceCollection.AddSingleton<IRemoteHashTable, RemoteHashTable>();
            serviceCollection.AddSingleton<IInMemoryHashTable, InMemoryHashTable>();

            return serviceCollection;
        }
    }
}
