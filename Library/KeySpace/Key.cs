using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.KeySpace
{
    public class Key : IComparable<Key>, IEquatable<Key>
    {
        public byte[] Value { get; }
        public string Base64EncodedKey { get; }

        private Key(byte[] value, string base64EncodedKey)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Base64EncodedKey = base64EncodedKey ?? throw new ArgumentNullException(nameof(base64EncodedKey));
        }

        public static Key Create(byte[] value)
        {
            return new Key(value, System.Convert.ToBase64String(value));
        }

        public int CompareTo(Key other)
        {
            if (other.Value.Length != Value.Length)
            {
                throw new InvalidOperationException("Keys are not from the same space");
            }

            for (var i = 0; i < Value.Length; ++i)
            {
                if (Value[i] == other.Value[i])
                {
                    continue;
                }

                return Value[i] - other.Value[i];
            }

            return 0;
        }

        public bool Equals(Key other)
        {
            if (other.Value.Length != Value.Length)
            {
                throw new InvalidOperationException("Keys are not from the same space");
            }

            return Value.SequenceEqual(other.Value);
        }

        public override string ToString()
        {
            return Base64EncodedKey;
        }
    }
}
