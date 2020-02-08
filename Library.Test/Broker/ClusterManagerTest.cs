using Library.Broker;
using Library.Model;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Library.Test.Broker
{
    public class ClusterManagerTest
    {
        [Test]
        public async Task AllScenarios()
        {
            var constantLiveness = new ConstantLivenessCheck(true);
            var curNode = new NodeIdentifier("dummy", Key.Create(new byte[] { 1 }));
            var nodeIdentifierFactory = new ConstantNodeIdentifierFactory(curNode);
            var clusterManager = new ClusterManager(constantLiveness, Mock.Of<ILogger<ClusterManager>>(), nodeIdentifierFactory);
            var newNode = new NodeIdentifier("dummy2", Key.Create(new byte[] { 2 }));
            var newNodeCopy = new NodeIdentifier("dummy2", Key.Create(new byte[] { 2 }));

            Assert.IsTrue(newNodeCopy.Equals(newNode));
            Assert.IsTrue(clusterManager.ContainsNode(curNode));
            Assert.IsFalse(clusterManager.ContainsNode(newNode));

            Assert.IsTrue(await clusterManager.AddNodeAsync(newNode, Address.Local));
            Assert.IsTrue(clusterManager.ContainsNode(newNode));
            Assert.IsTrue(clusterManager.ContainsNode(newNodeCopy));

            var removed = await clusterManager.FindDeadNodesAsync();
            Assert.AreEqual(0, removed.Count);

            constantLiveness.Liveness = false;
            removed = await clusterManager.FindDeadNodesAsync();
            Assert.AreEqual(1, removed.Count);

            clusterManager.RemoveNodes(removed);
            Assert.IsFalse(clusterManager.ContainsNode(newNode));
        }
    }
}
