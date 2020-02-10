using System.Threading.Tasks;

namespace Library.Broker
{
    public interface ILivenessCheck
    {
        Task<bool> IsAliveAsync(Address address);
    }
}
