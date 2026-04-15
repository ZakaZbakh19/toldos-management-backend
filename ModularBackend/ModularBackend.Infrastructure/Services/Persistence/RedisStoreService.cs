using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using ModularBackend.Application.Abstractions.Services.Persistence;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Services.Persistence
{
    public class RedisStoreService : IStoreService
    {
        private static readonly System.Text.Json.JsonSerializerOptions JsonOptions = new(System.Text.Json.JsonSerializerDefaults.Web);
        private readonly RedisOptions RedisOptions;
        private readonly IDatabase _database;

        public RedisStoreService(IConnectionMultiplexer multiplexer, IOptions<RedisOptions> options)
        {
            _database = multiplexer.GetDatabase();
            RedisOptions = options.Value;
        }

        public async Task<bool> ExistsAsync(string endpoint, string logic, string keyValue, CancellationToken cancellationToken = default)
        {
            var key = BuildKey(endpoint, logic, keyValue);
            return await _database.KeyExistsAsync(key);
        }

        public async Task DeleteAsync(string endpoint, string logic, string keyValue, CancellationToken cancellationToken = default)
        {
            // KeyDeleteAsync returns Task<bool> — await and discard result to match interface Task
            await _database.KeyDeleteAsync(BuildKey(endpoint, logic, keyValue));
        }

        private static string BuildKey(string endpoint, string logic, string key) => $"{endpoint}:{logic}:{key}";

        public async Task<T?> GetAsync<T>(string endpoint, string logic, string keyValue, CancellationToken cancellationToken = default) where T : class
        {
            var key = BuildKey(endpoint, logic, keyValue);
            var value = await _database.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return null;

            // disambiguate overloads by passing a string
            return System.Text.Json.JsonSerializer.Deserialize<T>(value.ToString(), JsonOptions);
        }

        public async Task SaveAsync<T>(string endpoint, string logic, string keyValue, T entity, TimeSpan ttl, CancellationToken cancellationToken = default) where T : class
        {
            var key = BuildKey(endpoint, logic, keyValue);

            var json = System.Text.Json.JsonSerializer.Serialize(entity, JsonOptions);

            await _database.StringSetAsync(key, json, ttl);
        }
    }
}
