using Library.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class NodeIdentifier : IEquatable<NodeIdentifier>
    {
        public static NodeIdentifier Random() => new NodeIdentifier(Guid.NewGuid().ToString(), Key.Create(Guid.NewGuid().ToByteArray()));
        public string Name { get; }

        public Key Key { get; }

        public NodeIdentifier(string name, Key key)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public override string ToString() => Name;

        public bool Equals(NodeIdentifier other)
        {
            return Key.Equals(other.Key) && Name.Equals(other.Name);
        }

        public override bool Equals(object obj)
        {
            if (obj is NodeIdentifier node)
            {
                return Equals(node);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}
