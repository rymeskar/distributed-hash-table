using Library.Model;
using NUnit.Framework;
using System;

namespace Library.Test
{
    [TestFixture]
    public class KeyTest
    {
        [Test]
        public void WhenDifferentSpaceThenThrows()
        {
            var key1 = Key.Create(new byte[] { 1 });
            var key2 = Key.Create(new byte[] { 1, 2 });

            Assert.Throws<InvalidOperationException>(() => key1.CompareTo(key2));
            Assert.Throws<InvalidOperationException>(() => key1.Equals(key2));
        }

        [Test]
        public void BothEqual()
        {
            var key1 = Key.Create(new byte[] { 1, 2 });
            var key2 = Key.Create(new byte[] { 1, 2 });

            Assert.AreEqual(0, key1.CompareTo(key2));
            Assert.IsTrue(key1.Equals(key2));
        }

        [Test]
        public void Greater()
        {
            var key1 = Key.Create(new byte[] { 2, 2 });
            var key2 = Key.Create(new byte[] { 1, 2 });

            Assert.AreEqual(1, key1.CompareTo(key2));
            Assert.IsFalse(key1.Equals(key2));
        }

        [Test]
        public void Smaller()
        {
            var key1 = Key.Create(new byte[] { 1, 1 });
            var key2 = Key.Create(new byte[] { 1, 2 });

            Assert.AreEqual(-1, key1.CompareTo(key2));
            Assert.IsFalse(key1.Equals(key2));
        }

        [Test]
        public void ProperlyStringified()
        {
            var key1 = Key.Create(new byte[] { 1, 1 });

            Assert.AreEqual("AQE=", key1.Base64EncodedKey);
        }
    }
}