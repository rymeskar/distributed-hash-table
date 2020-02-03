using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class NodeIdentifierFactory : INodeIdentifierFactory
    {
        private readonly IKeySpaceHash _hasher;

        public NodeIdentifierFactory(IKeySpaceHash hasher)
        {
            _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        }

        public NodeIdentifier Create()
        {
            var name = Environment.MachineName;
            var key = _hasher.Hash(name);
            return new NodeIdentifier(name, key);
        }
    }
}
