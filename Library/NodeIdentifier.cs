using Library.KeySpace;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class NodeIdentifier
    {
        public string Name { get; }

        public Key Key { get; }

        public NodeIdentifier(string name, Key key)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }
    }
}
