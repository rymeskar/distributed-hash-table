using NUnit.Framework;
using System;

namespace Library.Test
{
    [TestFixture]
    public class FileHashTableTest : HashTableTestBase
    {
        public override IHashTable CreateHashTable()
        {
            return new FileHashTable(Guid.NewGuid().ToString());
        }
    }
}
