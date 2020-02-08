using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Model
{
    public class NodeIdentifierComparer : IComparer<NodeIdentifier>
    {
        public int Compare(NodeIdentifier x, NodeIdentifier y)
        {
            return x.Key.CompareTo(y.Key);
        }
    }
}
