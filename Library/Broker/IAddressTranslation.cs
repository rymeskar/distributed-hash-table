using System.Threading.Tasks;

namespace Library.Broker
{
    public interface IAddressTranslation
    {
        public Task<Address> TranslateAsync(string logicalName);

    }
}
