using System;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared;
using Microsoft.WindowsAzure.Storage.Auth;

namespace storage
{
    class Program
    {
        static void Main(string[] args)
        {
            runAsync().Wait();
        }

        private static async Task runAsync()
        {
            var azureServiceTokenProvider = 
                new AzureServiceTokenProvider();
            var tokenCredential = new TokenCredential(
                await azureServiceTokenProvider
                    .GetAccessTokenAsync("https://storage.azure.com/"));
            var storageCredentials = 
                new StorageCredentials(tokenCredential);

            try
            {
                var cloudStorageAccount = new CloudStorageAccount(
                    storageCredentials, 
                    useHttps: true, 
                    accountName: "laaz203rbacmsistg", 
                    endpointSuffix: "core.windows.net");
                var cloudBlobClient = 
                    cloudStorageAccount.CreateCloudBlobClient();

                var cref = cloudBlobClient
                                .GetContainerReference("contribapp");
                cref.CreateIfNotExists();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }
    }
}
