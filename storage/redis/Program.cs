using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace LinuxAcademy.AZ200.Storage.RedisSample
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            var storage = new StorageFacade();
            var entity = await storage.GetEntityAsync("1", "Mike");
            if (entity == null)
            {
                entity = new Entity("1", "Mike", "Redis is cool");
                await storage.InsertOrUpdateEntityAsync(entity);
            }

            var entity2 = await storage.GetEntityAsync("1", "Mike");
            System.Console.WriteLine(entity2);
            var entity3 = await storage.GetEntityAsync("1", "Mike");
            System.Console.WriteLine(entity3);

            await Task.Delay(6000);

            var entity4 = await storage.GetEntityAsync("1", "Mike");
            System.Console.WriteLine(entity4);

            entity4.Description = "A new description";
            await storage.InsertOrUpdateEntityAsync(entity4);

            var entity5 = await storage.GetEntityAsync("1", "Mike");
            System.Console.WriteLine(entity5);
            var entity6 = await storage.GetEntityAsync("1", "Mike");
            System.Console.WriteLine(entity6);
        }
    }
}
