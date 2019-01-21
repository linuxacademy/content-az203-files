using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace LinuxAcademy.AZ200.StorageSamples
{
    public class Tables
    {
        public static async Task demosAsync()
        {
            
            var storageAccount = Common.getCloudStorageAccount();
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("Customers");
            await table.CreateIfNotExistsAsync();

            var customer = new CustomerEntity("Harp", "Walter")
            {
                Email = "Walter@contoso.com",
                PhoneNumber = "425-555-0101"
            };

            await InsertCustomerAsync(table, customer);
            await GetCustomerAsync(table, "Harp", "Walter");
            await InsertCustomerAsync(table, new CustomerEntity("Heydt", "Michael"));
            await InsertCustomerAsync(table, new CustomerEntity("Heydt", "Bleu"));
            await QueryCustomerAsync(table, "Heydt");
            var michael = await GetCustomerAsync(table, "Heydt", "Michael");
            michael.Email = "michael@localhost";
            await UpdateCustomerAsync(table, michael);
            //DeleteCustomerAsync(table, "Heydt", "Michael").Wait();


            await InsertCustomersAsync(table, new[] {
                new CustomerEntity("Doe", "John"),
                new CustomerEntity("Doe", "Jane")
                });

            await QueryCustomerAsync(table, "Doe");
        }

        private static async Task DeleteCustomerAsync(CloudTable table, string lastName, string firstName)
        {
            var customer = await GetCustomerAsync(table, lastName, firstName);
            var delete = TableOperation.Delete(customer);
            await table.ExecuteAsync(delete);
            Console.WriteLine("Deleted: " + customer);
        }

        private static async Task UpdateCustomerAsync(CloudTable table, CustomerEntity customer)
        {
            var update = TableOperation.Replace(customer);
            await table.ExecuteAsync(update);
        }

        private static async Task QueryCustomerAsync(CloudTable table, string lastName)
        {
            var query = new TableQuery<CustomerEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, lastName));
            var queryRsult = await table.ExecuteQuerySegmentedAsync(query, null);
            queryRsult.Results.ForEach(customer =>
            {
                Console.WriteLine(customer);
            });
        }

        static async Task<CustomerEntity> GetCustomerAsync(CloudTable table, string partitionKey, string rowKey)
        {
            var retrieve = TableOperation.Retrieve<CustomerEntity>(partitionKey, rowKey);
            var result = await table.ExecuteAsync(retrieve);
            //Console.WriteLine(result.Result);
            return (CustomerEntity)result.Result;
        }

        static async Task InsertCustomerAsync(CloudTable table, CustomerEntity customer)
        {
            var insert = TableOperation.Insert(customer);
            await table.ExecuteAsync(insert);
        }

        static async Task InsertCustomersAsync(CloudTable table, CustomerEntity[] customers)
        {
            var batch = new TableBatchOperation();

            customers.ToList().ForEach(batch.Insert);

            await table.ExecuteBatchAsync(batch);
        }
    }
}