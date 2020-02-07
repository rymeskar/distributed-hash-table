using Library.Model;

namespace Library.KeySpace
{
    public interface IKeySpaceManager
    {
        bool CanHandle(Key key);

        Address HandlingAddress(Key key);
    }
}