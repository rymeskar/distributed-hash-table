using Library.Model;

namespace Library
{
    public interface IKeySpaceHash
    {
        Key Hash(string name);
    }
}