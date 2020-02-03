using System.Threading.Tasks;

namespace DistributedHashTable
{
    public interface IDnsResolution
    {
        Task<string> ResolveAsync(string podName);
    }
}