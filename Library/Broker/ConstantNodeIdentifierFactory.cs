using System;
using System.Diagnostics.CodeAnalysis;

namespace Library.Broker
{
    [ExcludeFromCodeCoverage]
    public class ConstantNodeIdentifierFactory : INodeIdentifierFactory
    {
        private readonly NodeIdentifier _curNode;

        public ConstantNodeIdentifierFactory(NodeIdentifier curNode)
        {
            _curNode = curNode ?? throw new ArgumentNullException(nameof(curNode));
        }

        public NodeIdentifier Create()
        {
            return _curNode;
        }
    }
}
