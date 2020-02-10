using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Test
{
    [TestFixture]
    public class DistributedHashTableComponentTest
    {
        protected string KeyFactory() => Guid.NewGuid().ToString();
        protected string Value = Guid.NewGuid().ToString();


        // TODO: More complext behavior unit test!

        [Test]
        public void ThrowCases()
        {
            var key = KeyFactory();
            var hashTable = CreateHashTable();
            Assert.ThrowsAsync<KeyNotFoundException>(() => hashTable.GetAsync(key));
            Assert.DoesNotThrow(() => hashTable.RemoveAsync(key));
        }

        [Test]
        public async Task StoreGetRemoveGet()
        {
            var hashTable = CreateHashTable();

            var key = KeyFactory();
            await hashTable.StoreAsync(key, Value);
            var value = await hashTable.GetAsync(key);
            Assert.AreEqual(Value, value.Result.Value);
            await hashTable.RemoveAsync(key);
            Assert.ThrowsAsync<KeyNotFoundException>(() => hashTable.GetAsync(key));
        }

        public IDistributedHashTable CreateHashTable()
        {
            var di = new ServiceCollection();
            di.AddDistributedHashTable();
            di.AddSingleton(Mock.Of<IRemoteNodeHashTable>());
            di.AddLogging(builder => builder.AddConsole());
            return di.BuildServiceProvider().GetService<IDistributedHashTable>();
        }
    }
}
