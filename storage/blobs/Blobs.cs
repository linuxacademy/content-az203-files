using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace linuxacademy.az203.storage.blobs
{
    public class Blobs
    {
        public static string _connectionString = 
            "DefaultEndpointsProtocol=https;EndpointSuffix=core.windows.net;AccountName=laaz203blobs;AccountKey=3A1YdQ11kVDHJZy+V6lEw4ejKmPxLyUbKf86DCpffYV+/Ctt/vdLhG4/bKvs8vmgbJZHk4PORqB48H55BQpNlw==";
        public static async Task RunAsync()
        {
            var storageAccount = CloudStorageAccount
                                    .Parse(_connectionString);
            var cloudBlobClient = storageAccount
                                    .CreateCloudBlobClient();

            var cloudBlobContainer = cloudBlobClient
                                        .GetContainerReference("mycontainer");
            await cloudBlobContainer.CreateAsync();

            var permissions = new BlobContainerPermissions {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            await cloudBlobContainer.SetPermissionsAsync(permissions);

            var localFileName = "Blob.txt";
            File.WriteAllText(localFileName, "Hello, World!");

            var cloudBlockBlob = cloudBlobContainer
                                    .GetBlockBlobReference(localFileName);
            await cloudBlockBlob.UploadFromFileAsync(localFileName);

            // List the blobs in the container.
            Console.WriteLine("Listing blobs in container.");
            BlobContinuationToken blobContinuationToken = null;
            do {
                var results = await cloudBlobContainer
                        .ListBlobsSegmentedAsync(null, 
                                                 blobContinuationToken);
                blobContinuationToken = results.ContinuationToken;
                foreach (var item in results.Results) {
                    Console.WriteLine(item.Uri);
                }
            } while (blobContinuationToken != null); 

            var destinationFile = localFileName.Replace(".txt", "_DOWNLOADED.txt");
            await cloudBlockBlob.DownloadToFileAsync(
                                    destinationFile, FileMode.Create);

            var leaseId = Guid.NewGuid().ToString();

            File.WriteAllText(localFileName, "New Content");

            cloudBlockBlob.AcquireLease(
                TimeSpan.FromSeconds(30),
                leaseId);

            try
            {
                await cloudBlockBlob.UploadFromFileAsync(localFileName);
            }
            catch (StorageException ex)
            {
                System.Console.WriteLine(ex.Message);
                if (ex.InnerException != null) System.Console.WriteLine(ex.InnerException.Message);
            }

            await Task.Delay(TimeSpan.FromSeconds(5));

            await cloudBlockBlob.UploadFromFileAsync(localFileName);

            /*
            // or release it, but since we uploaded a new file, the lease is 
            await cloudBlockBlob.ReleaseLeaseAsync(new AccessCondition()
            {
                LeaseId = leaseId
            });
            */

            await cloudBlobContainer.DeleteIfExistsAsync();
        }
    }
}