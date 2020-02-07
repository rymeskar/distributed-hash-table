using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class Address
    {
        public string NetworkAddress { get; }

        public Address(string networkAddress)
        {
            NetworkAddress = networkAddress ?? throw new ArgumentNullException(nameof(networkAddress));
        }
    }
}
