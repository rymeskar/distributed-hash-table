using Library.KeySpace;
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

        protected Key Key = Key.Create(new byte[] { 1 });
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
            Assert.AreEqual(Value, value.Value);
            await hashTable.RemoveAsync(Key);
            Assert.ThrowsAsync<KeyNotFoundException>(() => hashTable.GetAsync(Key));
        }
    }
}
