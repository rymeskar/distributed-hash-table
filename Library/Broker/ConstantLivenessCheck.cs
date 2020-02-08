using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace Library.Broker
{
    [ExcludeFromCodeCoverage]
    public class ConstantLivenessCheck : ILivenessCheck
    {
        public bool Liveness { get; set; }

        public ConstantLivenessCheck(bool liveness)
        {
            Liveness = liveness;
        }

        public Task<bool> IsAliveAsync(Address address)
        {
            return Task.FromResult(Liveness);
        }
    }
}
