using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Model
{
    public class KeyComparer : IComparer<Key>
    {
        public int Compare(Key x, Key y)
        {
            return x.CompareTo(y);
        }
    }
}
