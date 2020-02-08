using Library.Model;
using System.Threading.Tasks;

namespace Library
{
    public interface IDistributedHashTable
    {
        Task<DistributedResult> GetAsync(string key);
        Task<NodeIdentifier> RemoveAsync(string key);
        Task<NodeIdentifier> StoreAsync(string key, string value);
    }
}