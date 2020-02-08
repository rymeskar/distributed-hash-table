﻿using Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.KeySpace
{
    public class DistributedKeySpaceManager : IKeySpaceManager
    {
        private SortedSet<NodeIdentifier> _orderedList;

        public DistributedKeySpaceManager(INodeIdentifierFactory nodeIdentifierFactory)
        {
            var nodeIdentifier = nodeIdentifierFactory.Create();
            _orderedList = new SortedSet<NodeIdentifier>(new NodeIdentifierComparer());
            _orderedList.Add(nodeIdentifier);
        }

        public void AddNode(NodeIdentifier node)
        {
            _orderedList.Add(node);
        }

        public NodeIdentifier GetHandlingNode(Key key)
        {
            if (_orderedList.Count == 0)
            {
                throw new InvalidOperationException("No nodes contained");
            }

            foreach (var node in _orderedList)
            {
                if (key.CompareTo(node.Key) <= 0)
                {
                    return node;
                }
            }
            
            return _orderedList.Last();
        }

        public void RemoveNodes(IList<NodeIdentifier> nodes)
        {
            foreach (var node in nodes)
            {
                _orderedList.Remove(node);
            }
        }
    }
}