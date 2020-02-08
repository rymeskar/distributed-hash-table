using Library.KeySpace;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library.Test
{
    [TestFixture]
    public class DistributedHashTableComponentTest
    {
        protected string Key = "123";
        protected string Value = "1";



        [Test]
        public void ThrowCases()
        {
            var hashTable = CreateHashTable();
            Assert.ThrowsAsync<KeyNotFoundException>(() => hashTable.GetAsync(Key));
            Assert.DoesNotThrow(() => hashTable.RemoveAsync(Key));
        }

        [Test]
        public async Task StoreGetRemoveGet()
        {
            var hashTable = CreateHashTable();

            await hashTable.StoreAsync(Key, Value);
            var value = await hashTable.GetAsync(Key);
            Assert.AreEqual(Value, value.Result.Value);
            await hashTable.RemoveAsync(Key);
            Assert.ThrowsAsync<KeyNotFoundException>(() => hashTable.GetAsync(Key));
        }

        public IDistributedHashTable CreateHashTable()
        {
            var di = new ServiceCollection();
            di.AddDistributedHashTable();
            return di.BuildServiceProvider().GetService<IDistributedHashTable>();
        }
    }
}
