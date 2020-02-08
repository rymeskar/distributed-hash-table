using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Model
{
    public class AddressableNode
    {
        public NodeIdentifier Node { get; }

        public Address Address { get; }

        public AddressableNode(NodeIdentifier node, Address address)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }
    }
}
