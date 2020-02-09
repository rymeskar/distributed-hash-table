using Library.HashTable;
using Library.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library.Test.HashTable
{
    class InMemoryExpiringHashTableTest : HashTableTestBase
    {
        public override IHashTable CreateHashTable()
        {
            return CreateHashTable(TimeSpan.FromMinutes(2));
        }

        public IHashTable CreateHashTable(TimeSpan expiry)
        {
            var options = new DHTOptions();
            options.InMemoryCaching = expiry;
            return new InMemoryExpiringHashTable(new MemoryCache(new MemoryCacheOptions()), Options.Create(options));
        }

        [Test]
        public async Task ExpiryWorks()
        {
            var hashTable = CreateHashTable(TimeSpan.FromMilliseconds(1));
            var value = "DUMMY";
            var key = Key.Create(Guid.NewGuid().ToByteArray());
            await hashTable.StoreAsync(key, value);
            await Task.Delay(10);

            Assert.ThrowsAsync<KeyNotFoundException>(() => hashTable.GetAsync(key));
        }
    }
}
