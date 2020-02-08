using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Model
{
    public class DistributedResult
    {
        public Result Result { get; }

        public NodeIdentifier Node { get; }

        public DistributedResult(Result result, NodeIdentifier node)
        {
            Result = result ?? throw new ArgumentNullException(nameof(result));
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }
    }
}
