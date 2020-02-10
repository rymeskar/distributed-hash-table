using System.Collections.Generic;

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
