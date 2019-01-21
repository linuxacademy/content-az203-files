using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace LaAz200Functions
{
    public static class OrderProcessor
    {
        [FunctionName("ProcessOrders")]
        public static void ProcessOrders(
            [QueueTrigger("incoming-orders", Connection = "AzureWebJobsStorage")]CloudQueueMessage myQueueItem,
            [Table("Orders")]ICollector<Order> tableBindings,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: ");
            log.LogInformation($"Processing Order: {myQueueItem.Id}");
            log.LogInformation($"Queue Insertion Time: {myQueueItem.InsertionTime}");
            log.LogInformation($"Queue Insertion Time: {myQueueItem.ExpirationTime}");
            //tableBindings.Add(JsonConvert.DeserializeObject<Order>(myQueueItem.AsString));
            throw new Exception("");
        
        }

        [FunctionName("ProcessOrders-Poison")]
        public static void ProcessFailedOrders([QueueTrigger("incoming-orders-poison", Connection = "AzureWebJobsStorage")]CloudQueueMessage myQueueItem, 
        ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}