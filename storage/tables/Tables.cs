using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace LinuxAcademy.AZ200.StorageSamples
{
    public class Tables
    {
        private static string _connectionString = "";

        public static async Task runDemoAsync(){
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();

            var gamersTable = tableClient.GetTableReference("Gamers");
            await gamersTable.CreateIfNotExistsAsync(); 

            await DeleteAllGamersAsync(gamersTable);

            var gamer1 = new Gamer("bleu@game.net", "France", "Bleu");
            await AddAsync(gamersTable, gamer1);

            var bleu = await GetAsync<Gamer>(gamersTable, "France", "bleu@game.net");
            System.Console.WriteLine(bleu);

            var gamers = new List<Gamer> {
                new Gamer("mike@game.net", "US", "Mike", "555-1212"),
                new Gamer("mike@contoso.net", "US", "Mike", "555-1234")
            };
            await AddBatchAsync(gamersTable, gamers);

            gamers = await FindGamersByNameAsync(gamersTable, "Mike");
            gamers.ForEach(Console.WriteLine);
        }

        public static async Task AddAsync<T>(CloudTable table, T entity) where T : TableEntity
        {
            var insertOperation = TableOperation.Insert(entity);
            await table.ExecuteAsync(insertOperation);
        }

        public static async Task AddBatchAsync<T>(CloudTable table, IEnumerable<T> entities) where T : TableEntity
        {
            var batchOperation = new TableBatchOperation();
            foreach (var entity in entities) batchOperation.Insert(entity);
            await table.ExecuteBatchAsync(batchOperation);
        }

        public static async Task<T> GetAsync<T>(CloudTable table, string pk, string rk) where T : TableEntity
        {
            var retrieve = TableOperation.Retrieve<Gamer>(pk, rk);
            var result = await table.ExecuteAsync(retrieve);
            return (T)result.Result;
        }

        public static async Task DeleteAsync<T>(CloudTable table, T entity) where T : TableEntity
        {
            var retrieve = TableOperation.Delete(entity);
            await table.ExecuteAsync(retrieve);
        }

        public static async Task<List<Gamer>> FindGamersByNameAsync(CloudTable table, string name) 
        {
            var filterCondition = TableQuery.GenerateFilterCondition("Name", QueryComparisons.Equal, name);
            var query = new TableQuery<Gamer>().Where(filterCondition);
            var results = await table.ExecuteQuerySegmentedAsync(query, null); // used to be ExecuteQuery / ExecuteQueryAsync
            return results.ToList();
        }

        public static async Task DeleteAllGamersAsync(CloudTable table)
        {
            // believe it or now, this is a hard problem to do right.  This is a fudge for this example
            var gamers = new [] {
                await GetAsync<Gamer>(table, "US", "mike@game.net"),
                await GetAsync<Gamer>(table, "US", "mike@contoso.net"),
                await GetAsync<Gamer>(table, "France", "bleu@game.net")
            }.ToList();
            gamers.ForEach(async gamer => 
            {
                if (gamer != null)
                    await DeleteAsync(table, gamer);
            });
        }
    }
}
