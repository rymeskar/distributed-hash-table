using Library.Model;
using System.Collections.Generic;

namespace Library.KeySpace
{
    public interface IKeySpaceManager
    {
        NodeIdentifier GetHandlingNode(Key key);

        void AddNode(NodeIdentifier node);

        void RemoveNodes(IList<NodeIdentifier> node);
    }
}