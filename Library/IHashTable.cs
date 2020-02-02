using Library.KeySpace;
using System.Threading.Tasks;

namespace Library
{
    public interface IHashTable
    {
        Task<Result> GetAsync(Key key);
        Task RemoveAsync(Key key);
        Task StoreAsync(Key key, string value);
    }
}