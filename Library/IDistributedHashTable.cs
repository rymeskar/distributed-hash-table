using System.Threading.Tasks;

namespace Library
{
    public interface IDistributedHashTable
    {
        Task<Result> GetAsync(string key);
        Task RemoveAsync(string key);
        Task StoreAsync(string key, string value);
    }
}