using NUnit.Framework;

namespace Library.Test
{
    [TestFixture]
    public class InMemoryPermanentHashTableTest : HashTableTestBase
    {
        public override IHashTable CreateHashTable()
        {
            return new InMemoryPermanentHashTable();
        }
    }
}
