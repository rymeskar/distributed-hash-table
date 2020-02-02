﻿using DistributedHashTable;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DistributedHashTable.HashTable;

namespace DistributedHashTableClient
{
    public class DHTFunctionalTest
    {
        private readonly string Key = "123";
        private readonly string Value = "1";

        private readonly HashTableClient _client;
        private readonly ILogger<DHTFunctionalTest> _logger;

        public DHTFunctionalTest(HashTableClient client, ILogger<DHTFunctionalTest> logger)
        {
            this._client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger;
        }

        public async Task ThrowCases()
        {
            try
            {
                await _client.GetAsync(GRPCHelpers.CreateGetRequest(Key));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while Getting Key");
            }
        }
        public Task StoreGet() => StoreGet(Key, Value);

        private async Task StoreGet(string key, string value)
        {
            await _client.StoreAsync(GRPCHelpers.CreateStoreRequest(key, value));
            var response = await _client.GetAsync(GRPCHelpers.CreateGetRequest(Key));

            Log(response, value);
        }

        public async Task RandomTest()
        {
            var tasks = Enumerable.Range(0, 100).Select(t => StoreGet(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            await Task.WhenAll(tasks);
        }
        public void Log(ValueResponse response, string originalValue)
        {

            if (originalValue != response.Value)
            {
                _logger.LogError($"Response does not match! {response.Value}; original: {originalValue}");
            }

            _logger.LogInformation($"Respone: {response.Value}, Info: {response.Info.Identifier}");
        }
    }
}
