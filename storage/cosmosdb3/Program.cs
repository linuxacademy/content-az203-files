using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace cosmosdb
{
    class Program
    {
        private DocumentClient _client;

        static void Main(string[] args)
        {
            try
            {
                var p = new Program();
                p.BasicOperationsAsync().Wait();
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                //Console.ReadKey();
            }
        }

        private async Task BasicOperationsAsync()
        {
            _client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["accountEndpoint"]), ConfigurationManager.AppSettings["accountKey"]);

            await _client.CreateDatabaseIfNotExistsAsync(new Database { Id = "Users" });

            await _client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("Users"), new DocumentCollection { Id = "Orders" });

            await CreateEntitiesAsync();

            var user = await ReadUserDocumentAsync("Users", "WebCustomers", 1);
            user.LastName = "Suh";
            await ReplaceUserDocumentAsync("Users", "WebCustomers", user);

            await DeleteUserDocumentAsync("Users", "WebCustomers", 1);

            Console.WriteLine("Database and collection validation complete");
        }

        private void WriteToConsoleAndPromptToContinue(string format, params object[] args)
        {
            Console.WriteLine(format, args);
            Console.WriteLine("Press any key to continue ...");
            //Console.ReadKey();
        }

        private async Task CreateUserDocumentIfNotExistsAsync(string databaseName, string collectionName, User user)
        {
            try
            {
                await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, user.Id), new RequestOptions { PartitionKey = new PartitionKey(user.UserId) });
                WriteToConsoleAndPromptToContinue("User {0} already exists in the database", user.Id);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), user);
                    WriteToConsoleAndPromptToContinue("Created User {0}", user.Id);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateEntitiesAsync()
        {
            var yanhe = new User
            {
                Id = "1",
                UserId = "yanhe",
                LastName = "He",
                FirstName = "Yan",
                Email = "yanhe@contoso.com",
                OrderHistory = new OrderHistory[]
                    {
                        new OrderHistory {
                            OrderId = "1000",
                            DateShipped = "08/17/2018",
                            Total = "52.49"
                        }
                    },
                ShippingPreference = new ShippingPreference[]
                    {
                            new ShippingPreference {
                                    Priority = 1,
                                    AddressLine1 = "90 W 8th St",
                                    City = "New York",
                                    State = "NY",
                                    ZipCode = "10001",
                                    Country = "USA"
                            }
                    },
            };

            await CreateUserDocumentIfNotExistsAsync("Users", "WebCustomers", yanhe);

            var nelapin = new User
            {
                Id = "2",
                UserId = "nelapin",
                LastName = "Pindakova",
                FirstName = "Nela",
                Email = "nelapin@contoso.com",
                Dividend = "8.50",
                OrderHistory = new OrderHistory[]
                {
                    new OrderHistory {
                        OrderId = "1001",
                        DateShipped = "08/17/2018",
                        Total = "105.89"
                    }
                },
                ShippingPreference = new ShippingPreference[]
                {
                    new ShippingPreference {
                            Priority = 1,
                            AddressLine1 = "505 NW 5th St",
                            City = "New York",
                            State = "NY",
                            ZipCode = "10001",
                            Country = "USA"
                    },
                    new ShippingPreference {
                            Priority = 2,
                            AddressLine1 = "505 NW 5th St",
                            City = "New York",
                            State = "NY",
                            ZipCode = "10001",
                            Country = "USA"
                    }
                },
                Coupons = new CouponsUsed[]
                {
                    new CouponsUsed{
                        CouponCode = "Fall2018"
                    }
                }
            };

            await CreateUserDocumentIfNotExistsAsync("Users", "WebCustomers", nelapin);
        }

        private async Task<User> ReadUserDocumentAsync(string databaseName, string collectionName, int userId)
        {
            try
            {
                var result = await _client.ReadDocumentAsync(
                    UriFactory.CreateDocumentUri(databaseName, collectionName, userId.ToString()), 
                    new RequestOptions { PartitionKey = new PartitionKey(userId) }
                );

                WriteToConsoleAndPromptToContinue("Read user {0}", userId);

                return null;
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    WriteToConsoleAndPromptToContinue("User {0} not read", userId);
                }
                throw;
            }
        }

        private async Task ReplaceUserDocumentAsync(string databaseName, string collectionName, User updatedUser)
        {
            try
            {
                await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, updatedUser.Id), updatedUser, new RequestOptions { PartitionKey = new PartitionKey(updatedUser.UserId) });
                WriteToConsoleAndPromptToContinue("Replaced last name for {0}", updatedUser.LastName);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    WriteToConsoleAndPromptToContinue("User {0} not found for replacement", updatedUser.Id);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task DeleteUserDocumentAsync(string databaseName, string collectionName, int userToDeleteId)
        {
            try
            {
                await _client.DeleteDocumentAsync(
                    UriFactory.CreateDocumentUri(databaseName, collectionName, userToDeleteId.ToString()), 
                    new RequestOptions { PartitionKey = new PartitionKey(userToDeleteId) });
                Console.WriteLine("Deleted user {0}", userToDeleteId);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    WriteToConsoleAndPromptToContinue("User {0} not found for deletion", userToDeleteId);
                }
                else
                {
                    throw;
                }
            }
        }

        private void ExecuteSimpleQuery(string databaseName, string collectionName)
        {
            // Set some common query options
            var queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };

            // Here we find nelapin via their LastName
            var userQuery = _client.CreateDocumentQuery<User>(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions)
                    .Where(u => u.LastName == "Pindakova");

            // The query is executed synchronously here, but can also be executed asynchronously via the IDocumentQuery<T> interface
            Console.WriteLine("Running LINQ query...");
            foreach (var user in userQuery)
            {
                Console.WriteLine("\tRead {0}", user);
            }

            // Now execute the same query via direct SQL
            var userQueryInSql = _client.CreateDocumentQuery<User>(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName),
                    "SELECT * FROM User WHERE User.lastName = 'Pindakova'", queryOptions );

            Console.WriteLine("Running direct SQL query...");
            foreach (var user in userQueryInSql)
            {
                    Console.WriteLine("\tRead {0}", user);
            }

            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }
    }
}