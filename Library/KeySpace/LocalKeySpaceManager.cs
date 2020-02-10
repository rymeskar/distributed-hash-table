using Library.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Library.KeySpace
{
    [ExcludeFromCodeCoverage]
    public class LocalKeySpaceManager : IKeySpaceManager
    {
        private NodeIdentifier _nodeIdentifier;

        public LocalKeySpaceManager(INodeIdentifierFactory nodeIdentifierFactory)
        {
            _nodeIdentifier = nodeIdentifierFactory.Create();
        }

        public void AddNode(NodeIdentifier node)
        {
            throw new NotImplementedException();
        }

        public NodeIdentifier GetHandlingNode(Key key)
        {
            return _nodeIdentifier;
        }

        public void RemoveNodes(IList<NodeIdentifier> node)
        {
            throw new NotImplementedException();
        }
    }
}
