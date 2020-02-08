using System.Threading.Tasks;

namespace Library.Broker
{
    public interface IClusterBrokerService
    {
        Task KeepUpToDate();
        Task<bool> TryAddNode(NodeIdentifier node);
    }
}