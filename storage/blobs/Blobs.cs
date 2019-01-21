using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace LinuxAcademy.AZ200.StorageSamples
{
    public class Blobs
    {
        private static async Task ProcessAsync()
        {
            var storageConnectionString = Environment.GetEnvironmentVariable("storageconnectionstring");

            CloudStorageAccount storageAccount = null;
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount)) {
                try {
                    var cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    var cloudBlobContainer = cloudBlobClient.GetContainerReference("mycontainer" + Guid.NewGuid().ToString());
                    await cloudBlobContainer.CreateAsync();

                    var permissions = new BlobContainerPermissions {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    var localPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string localFileName = "QuickStart_" + Guid.NewGuid().ToString() + ".txt";
                    sourceFile = Path.Combine(localPath, localFileName);
                    File.WriteAllText(sourceFile, "Hello, World!");

                    var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(localFileName);
                    await cloudBlockBlob.UploadFromFileAsync(sourceFile);

                    // List the blobs in the container.
                    Console.WriteLine("Listing blobs in container.");
                    BlobContinuationToken blobContinuationToken = null;
                    do {
                        var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                        blobContinuationToken = results.ContinuationToken;
                        foreach (IListBlobItem item in results.Results) {
                            Console.WriteLine(item.Uri);
                        }
                    } while (blobContinuationToken != null); // Loop while the continuation token is not null.

                    destinationFile = sourceFile.Replace(".txt", "_DOWNLOADED.txt");
                    await cloudBlockBlob.DownloadToFileAsync(destinationFile, FileMode.Create);
                }
                catch (StorageException ex)
                {
                    Console.WriteLine("Error returned from the service: {0}", ex.Message);
                }
                finally
                {
                    Console.WriteLine("Press any key to delete the sample files and example container.");
                    Console.ReadLine();
                    // Clean up resources. This includes the container and the two temp files.
                    Console.WriteLine("Deleting the container and any blobs it contains");
                    if (cloudBlobContainer != null)
                    {
                        await cloudBlobContainer.DeleteIfExistsAsync();
                    }
                    Console.WriteLine("Deleting the local source file and local downloaded files");
                    Console.WriteLine();
                    File.Delete(sourceFile);
                    File.Delete(destinationFile);
                }
            }
            else
            {
                Console.WriteLine(
                    "A connection string has not been defined in the system environment variables. " +
                    "Add a environment variable named 'storageconnectionstring' with your storage " +
                    "connection string as a value.");
            }
        }
    }
}