using System;
using System.Configuration;
using StackExchange.Redis;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace LinuxAcademy.AZ200.Storage.RedisSample
{
    public class StorageFacade
    {
        private static Lazy<CloudStorageAccount> _lazyTableStorageAccount =
            new Lazy<CloudStorageAccount>(() =>
            {
                var tableStorageConnection = ConfigurationManager.AppSettings["TableConnection"];
                return CloudStorageAccount.Parse(tableStorageConnection);
            });
        public CloudStorageAccount TableStorageAccount => _lazyTableStorageAccount.Value;

        private static Lazy<ConnectionMultiplexer> _lazyCacheConnection =
            new Lazy<ConnectionMultiplexer>(() =>
            {
                var cacheConnection = ConfigurationManager.AppSettings["CacheConnection"];
                return ConnectionMultiplexer.Connect(cacheConnection);
            });
        public static ConnectionMultiplexer Connection => _lazyCacheConnection.Value;

        public StorageFacade()
        {
        }

        // Set five minute expiration as a default
        private const double CacheExpirationTimeInSeconds = 5;

        public async Task<Entity> GetEntityAsync(string pk, string rk)
        {
            // Define a unique key for this method and its parameters.
            var key = $"Entity:{pk}_{rk}";
            var cache = Connection.GetDatabase();
            
            // Try to get the entity from the cache.
            var json = await cache.StringGetAsync(key);
            var entityFromCache = string.IsNullOrWhiteSpace(json) 
                        ? default(Entity) 
                        : JsonConvert.DeserializeObject<Entity>(json);

            if (entityFromCache == null) // Cache miss
            {
                // If there's a cache miss, get the entity from the original store and cache it.
                // Code has been omitted because it's data store dependent.  
                Console.WriteLine("Cache miss");
                
                var tableClient = TableStorageAccount.CreateCloudTableClient();
                var table = tableClient.GetTableReference("Entities");
                await table.CreateIfNotExistsAsync();
                var retrieveOperation = TableOperation.Retrieve<Entity>(pk, rk);
                var result = await table.ExecuteAsync(retrieveOperation);
                var entityFromTable = (Entity)result.Result;

                // Avoid caching a null value.
                if (entityFromTable != null)
                {
                    // Put the item in the cache with a custom expiration time that 
                    // depends on how critical it is to have stale data.
                    await cache.StringSetAsync(key, entityFromTable.ToString());
                    await cache.KeyExpireAsync(key, TimeSpan.FromSeconds(CacheExpirationTimeInSeconds));
                    Console.WriteLine("Cached new entity");
                }
                else
                {
                    Console.WriteLine("Entity not found in backend - not caching null");
                }

                return entityFromTable;
            }
            else
            {
                Console.WriteLine("Found in cache");
            }

            return entityFromCache;
        }

        public async Task InsertOrUpdateEntityAsync(Entity entity)
        {
            // Update the object in the original data store.
            var tableClient = TableStorageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("Entities");
            await table.CreateIfNotExistsAsync();
            var insertOrReplaceOperation = TableOperation.InsertOrReplace(entity);
            await table.ExecuteAsync(insertOrReplaceOperation);

            // Invalidate the current cache object.
            var cache = Connection.GetDatabase();
            var key = $"Entity:{entity.PartitionKey}_{entity.RowKey}";
            await cache.KeyDeleteAsync(key); // Delete this key from the cache.

            Console.WriteLine("Updated backend and deleted key from cache");
        }
    }
}