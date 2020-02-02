using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class NodeIdentifier
    {
        public string Id { get; }

        public NodeIdentifier(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }
    }
}
