using Library.Broker;
using Library.KeySpace;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Test.Broker
{
    [TestFixture]
    public class ClusterBrokerServiceTest
    {
        [Test]
        public async Task AlreadyAddedReturnsFalse()
        {
            var clusterManagerMock = new Mock<IClusterManager>(MockBehavior.Strict);
            clusterManagerMock.Setup(s => s.ContainsNode(It.IsAny<NodeIdentifier>())).Returns(true);
            var brokerService = new ClusterBrokerService(clusterManagerMock.Object, Mock.Of<IAddressTranslation>(), Mock.Of<IKeySpaceManager>());
            
            Assert.IsFalse(await brokerService.TryAddNode(NodeIdentifier.Random()));
        }

        [Test]
        public async Task WhenShouldAddThenAdds()
        {
            var clusterManagerMock = new Mock<IClusterManager>(MockBehavior.Strict);
            clusterManagerMock.Setup(s => s.ContainsNode(It.IsAny<NodeIdentifier>())).Returns(false);
            clusterManagerMock.Setup(s => s.AddNodeAsync(It.IsAny<NodeIdentifier>(), It.IsAny<Address>())).Returns(Task.FromResult(true));
            var brokerService = new ClusterBrokerService(clusterManagerMock.Object, Mock.Of<IAddressTranslation>(), Mock.Of<IKeySpaceManager>());

            Assert.IsTrue(await brokerService.TryAddNode(NodeIdentifier.Random()));
        }

        [Test]
        public async Task WhenShouldButNotAliveThenDoesNotAdd()
        {
            var clusterManagerMock = new Mock<IClusterManager>(MockBehavior.Strict);
            clusterManagerMock.Setup(s => s.ContainsNode(It.IsAny<NodeIdentifier>())).Returns(false);
            clusterManagerMock.Setup(s => s.AddNodeAsync(It.IsAny<NodeIdentifier>(), It.IsAny<Address>())).Returns(Task.FromResult(false));
            var brokerService = new ClusterBrokerService(clusterManagerMock.Object, Mock.Of<IAddressTranslation>(), Mock.Of<IKeySpaceManager>());

            Assert.IsFalse(await brokerService.TryAddNode(NodeIdentifier.Random()));
        }

        [Test]
        public async Task KeepUpToDateReturnsProper()
        {
            var clusterManagerMock = new Mock<IClusterManager>(MockBehavior.Strict);
            IList<NodeIdentifier> deadNodes = new List<NodeIdentifier>
            {
                NodeIdentifier.Random(),
                NodeIdentifier.Random()
            };

            clusterManagerMock.Setup(s => s.FindDeadNodesAsync()).Returns(Task.FromResult(deadNodes));
            clusterManagerMock.Setup(s => s.RemoveNodes(deadNodes));

            var keySpaceManagerMock = new Mock<IKeySpaceManager>(MockBehavior.Strict);
            keySpaceManagerMock.Setup(s => s.RemoveNodes(deadNodes));
            var brokerService = new ClusterBrokerService(clusterManagerMock.Object, Mock.Of<IAddressTranslation>(), keySpaceManagerMock.Object);
            await brokerService.KeepUpToDate();

            clusterManagerMock.VerifyAll();
            keySpaceManagerMock.VerifyAll();
        }
    }
}
