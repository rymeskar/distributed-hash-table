using System;
using System.Net;

namespace Library
{
    public class Address
    {
        public static Address Local = new Address(true);

        public string NetworkAddress { get; }

        public bool IsLocal { get; }

        public Address(string networkAddress)
        {
            NetworkAddress = networkAddress ?? throw new ArgumentNullException(nameof(networkAddress));
            IsLocal = false;
        }

        private Address(bool isLocal)
        {
            IsLocal = isLocal;
            NetworkAddress = IPAddress.Loopback.ToString();
        }
    }
}
