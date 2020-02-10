using Library.Broker;
using Library.KeySpace;
using Library.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Test
{
    public class DistributedKeySpaceManagerTest
    {
        [Test]
        public async Task AllScenarios()
        {
            var key1 = Key.Create(1);
            var constantNode = new ConstantNodeIdentifierFactory(CreateNode(key1));
            var manager = new DistributedKeySpaceManager(constantNode);

            Assert.AreEqual(key1, manager.GetHandlingNode(Key.Create(0)).Key);
            Assert.AreEqual(key1, manager.GetHandlingNode(Key.Create(2)).Key);

            var key10 = Key.Create(10);
            manager.AddNode(CreateNode(key10));

            Assert.AreEqual(key1, manager.GetHandlingNode(Key.Create(0)).Key);
            Assert.AreEqual(key10, manager.GetHandlingNode(Key.Create(2)).Key);
            Assert.AreEqual(key10, manager.GetHandlingNode(Key.Create(11)).Key);

            var key2 = Key.Create(2);
            manager.AddNode(CreateNode(key2));

            Assert.AreEqual(key1, manager.GetHandlingNode(Key.Create(0)).Key);
            Assert.AreEqual(key2, manager.GetHandlingNode(Key.Create(2)).Key);
            Assert.AreEqual(key10, manager.GetHandlingNode(Key.Create(11)).Key);

            manager.RemoveNodes(new List<NodeIdentifier> { CreateNode(key2), CreateNode(key10)});

            Assert.AreEqual(key1, manager.GetHandlingNode(Key.Create(0)).Key);
            Assert.AreEqual(key1, manager.GetHandlingNode(Key.Create(2)).Key);
        }

        private NodeIdentifier CreateNode(Key key) => new NodeIdentifier(Guid.NewGuid().ToString(), key);
    }
}
