using Library.KeySpace;

namespace Library
{
    public interface IKeySpaceHash
    {
        Key Hash(string name);
    }
}