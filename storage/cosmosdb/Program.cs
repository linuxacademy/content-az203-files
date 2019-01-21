using System;
using System.IO;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel; 

namespace LinuxAcademy.AZ200.CosmosDbSample
{ 
    class Program
    {
        private static DocumentClient _client;
        private const string _databaseId = "SqlSample";
        private const string _collectionId = "Families";

        static void Main(string[] args)
        {
            _client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["accountEndpoint"]), 
                                                 ConfigurationManager.AppSettings["accountKey"]);

            CreateDataAsync(_databaseId, _collectionId).Wait(); 
            ExecuteSqlQuery(_databaseId, _collectionId, 
            @"
SELECT *
FROM Families f
WHERE f.id = 'AndersenFamily'
");
            ExecuteSqlQuery(_databaseId, _collectionId, 
            @"
SELECT {""Name"":f.id, ""City"":f.address.city} AS Family
    FROM Families f
    WHERE f.address.city = f.address.state
");  
            ExecuteSqlQuery(_databaseId, _collectionId, 
            @"
SELECT c.givenName
    FROM Families f
    JOIN c IN f.children
    WHERE f.id = 'WakefieldFamily'
    ORDER BY f.address.city ASC");  
        }

        private static async Task CreateDataAsync(string databaseId, string collectionId)
        {
            await _client.CreateDatabaseIfNotExistsAsync(
                new Database { 
                    Id = databaseId
                });

            await _client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(databaseId), 
                new DocumentCollection 
                { 
                    Id = collectionId,
                    PartitionKey = new PartitionKeyDefinition() { 
                        Paths = new Collection<string>(new [] { "/id" })
                    }
                });

            var family1 = JObject.Parse(File.ReadAllText("data/andersen.json"));
            var family2 = JObject.Parse(File.ReadAllText("data/wakefield.json"));

            await CreateDocumentIfNotExistsAsync(databaseId, collectionId, family1["id"].ToString(), family1);
            await CreateDocumentIfNotExistsAsync(databaseId, collectionId, family2["id"].ToString(), family2);
        }

        private static async Task CreateDocumentIfNotExistsAsync(
            string databaseId, 
            string collectionId,
            string documentId,
            JObject data)
        {
            try
            {
                await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(
                    databaseId, collectionId, documentId),
                    new RequestOptions { 
                        PartitionKey = new PartitionKey(documentId) 
                    });
                Console.WriteLine($"Order {documentId} already exists in the database");
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await _client.CreateDocumentAsync(
                        UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), data);
                    Console.WriteLine($"Created Order {documentId}");
                }
                else
                {
                    throw;
                }
            }
        }

        private static void ExecuteSqlQuery(string databaseId, string collectionId, string sql)
        {
            System.Console.WriteLine("SQL: " + sql);
            // Set some common query options
            var queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };

            // Here we find nelapin via their LastName
            var sqlQuery = _client.CreateDocumentQuery<JObject>(
                    UriFactory.CreateDocumentCollectionUri(databaseId, collectionId),
                    sql, queryOptions );

            foreach (var result in sqlQuery)
            {
                Console.WriteLine(result);
            }
        }


        private static async Task<string> GetDocumentByIdAsync(
            string databaseId, 
            string collectionId,
            string documentId)
        {
            var response = await _client.ReadDocumentAsync(
                UriFactory.CreateDocumentUri(databaseId, collectionId, documentId),
                new RequestOptions { 
                    PartitionKey = new PartitionKey(Undefined.Value) 
                }
            );

            Console.WriteLine(response.Resource);

            return "";
        }

    }
}