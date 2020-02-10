using NUnit.Framework;

namespace Library.Test
{
    [TestFixture]
    public class RemoteHashTableTest : HashTableTestBase
    {
        public override IHashTable CreateHashTable()
        {
            return new RemoteHashTable();
        }
    }
}
