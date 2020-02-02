using Library.KeySpace;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Library
{
    public class KeySpaceHash : IKeySpaceHash
    {
        public Key Hash(string name)
        {
            using var sha = new SHA1CryptoServiceProvider();
            var result = sha.ComputeHash(Encoding.ASCII.GetBytes(name));
            return Key.Create(result);
        }
    }
}
