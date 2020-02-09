using Library.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Library
{
    public interface IFileHashTable : IHashTable { }

    public class FileHashTable : IFileHashTable
    {
        private readonly string _fileName;

        public FileHashTable(string fileName)
        {
            _fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        }

        public async Task<Result> GetAsync(Key key)
        {
            var hashTable = await CreateTemporaryHashTable();
            var retVal = await hashTable.GetAsync(key);
            retVal.MemorySource = "File";
            return retVal;
        }

        public async Task RemoveAsync(Key key)
        {
            var hashTable = await CreateTemporaryHashTable();
            await hashTable.RemoveAsync(key);
            await SaveHashTable(hashTable);
        }

        public async Task StoreAsync(Key key, string value)
        {
            var hashTable = await CreateTemporaryHashTable();
            await hashTable.StoreAsync(key, value);
            await SaveHashTable(hashTable);
        }

        private async Task<InMemoryPermanentHashTable> CreateTemporaryHashTable()
        {
            try
            {
                using var fs = File.OpenRead(_fileName);
                var text = File.ReadAllText(_fileName);
                var deserialized = await JsonSerializer.DeserializeAsync<IDictionary<string, string>>(fs);

                return InMemoryPermanentHashTable.Create(deserialized);
            }
            catch
            {
                return new InMemoryPermanentHashTable();
            }
        }

        private async Task SaveHashTable(InMemoryPermanentHashTable table)
        {
            using var fs = File.Create(_fileName);
            await JsonSerializer.SerializeAsync(fs, table.KeyToValue);
        }
    }
}
