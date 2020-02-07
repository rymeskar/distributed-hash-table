using Library.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public interface IInMemoryHashTable : IHashTable { }

    public class InMemoryHashTable : IInMemoryHashTable
    {
        public IDictionary<string, string> KeyToValue { get; }

        public InMemoryHashTable()
        {
            KeyToValue = new ConcurrentDictionary<string, string>();
        }

        private InMemoryHashTable(ConcurrentDictionary<string, string> keyToValue)
        {
            KeyToValue = keyToValue ?? throw new ArgumentNullException(nameof(keyToValue));
        }

        public static InMemoryHashTable Create(IDictionary<string, string> items)
        {
            var dict = new ConcurrentDictionary<string, string>(items);
            return new InMemoryHashTable(dict);
        }

        public Task StoreAsync(Key key, string value)
        {
            KeyToValue[key.Base64EncodedKey] = value;
            return Task.CompletedTask;
        }

        public Task<Result> GetAsync(Key key)
        {
            var value = KeyToValue[key.Base64EncodedKey];

            return Task.FromResult(new Result(value, "InMemory"));
        }

        public Task RemoveAsync(Key key)
        {
            KeyToValue.Remove(key.Base64EncodedKey);
            return Task.CompletedTask;
        }
    }
}
