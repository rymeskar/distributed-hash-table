using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library.Broker
{
    public interface ILivenessCheck
    {
        Task<bool> IsAliveAsync(Address address);
    }
}
