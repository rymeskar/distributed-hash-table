using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Test
{
    [TestFixture]
    public class InMemoryHashTableTest : HashTableTestBase
    {
        public override IHashTable CreateHashTable()
        {
            return new InMemoryHashTable();
        }
    }
}
