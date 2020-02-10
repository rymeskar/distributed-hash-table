using System.Collections.Generic;

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
