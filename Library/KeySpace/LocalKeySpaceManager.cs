using Library.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.KeySpace
{
    public class LocalKeySpaceManager : IKeySpaceManager
    {
        public bool CanHandle(Key key)
        {
            return true;
        }

        public Address HandlingAddress(Key key)
        {
            throw new NotImplementedException();
        }
    }
}
