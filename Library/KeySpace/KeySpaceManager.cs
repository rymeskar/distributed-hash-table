using System;
using System.Collections.Generic;
using System.Text;

namespace Library.KeySpace
{
    public class KeySpaceManager : IKeySpaceManager
    {
        public bool CanHandle(Key key)
        {
            return true;
        }
    }
}
