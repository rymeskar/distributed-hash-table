using Library.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library.Test
{
    public abstract class HashTableTestBase
    {
        public abstract IHashTable CreateHashTable();

        protected Key KeyFactory() => Key.Create(new byte[] { (byte) new Random().Next(), (byte)new Random().Next() });
        protected string ValueFactory() => Guid.NewGuid().ToString();

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
            var value = ValueFactory();

            await hashTable.StoreAsync(key, value);
            var actualValue = await hashTable.GetAsync(key);
            Assert.AreEqual(value, actualValue.Value);
            await hashTable.RemoveAsync(key);
            Assert.ThrowsAsync<KeyNotFoundException>(() => hashTable.GetAsync(key));
        }
    }
}
