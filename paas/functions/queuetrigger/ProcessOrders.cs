using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace linuxacademy.az203.paas.functions
{
    public static class OrderProcessor
    {
        [FunctionName("ProcessOrders")]
        public static void ProcessOrders(
            [QueueTrigger("incoming-orders", 
                        Connection = "AzureWebJobsStorage")]
            CloudQueueMessage queueItem,
            [Table("Orders", 
                Connection = "AzureWebJobsStorage")]
            ICollector<Order> tableBindings,
            ILogger log)
        {
            log.LogInformation($"Processing Order (mesage Id): {queueItem.Id}");
            log.LogInformation($"Processing at: {DateTime.UtcNow}");
            log.LogInformation($"Queue Insertion Time: {queueItem.InsertionTime}");
            log.LogInformation($"Queue Insertion Time: {queueItem.ExpirationTime}");
            log.LogInformation($"Data: {queueItem.AsString}");
            tableBindings.Add(JsonConvert.DeserializeObject<Order>(
                queueItem.AsString));
        }

        [FunctionName("ProcessOrders-Poison")]
        public static void ProcessFailedOrders(
            [QueueTrigger("incoming-orders-poison", 
                          Connection = "AzureWebJobsStorage")]
            CloudQueueMessage queueItem, 
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {queueItem}");
            log.LogInformation($"Data: {queueItem.AsString}");
        }
    }
}