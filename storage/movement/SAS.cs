using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage; // Namespace for Storage Client Library
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Azure Blobs
using Microsoft.WindowsAzure.Storage.File; // Namespace for Azure Files

namespace LinuxAcademy.AZ200.StorageSamples
{
    public class SAS
    {
        public static async Task demoFileSas()
        {
            var storageAccount = Common.getCloudStorageAccount();
            var fileClient = storageAccount.CreateCloudFileClient();
            var fileShare = fileClient.GetShareReference("logs");
            if (await fileShare.ExistsAsync())
            {
                // Create a new shared access policy and define its constraints.
                var sharedPolicy = new SharedAccessFilePolicy()
                    {
                        SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                        Permissions = SharedAccessFilePermissions.Read | SharedAccessFilePermissions.Write
                    };

                // Get existing permissions for the share.
                var permissions = await fileShare.GetPermissionsAsync();

                var policyName = "filePolicy";
                // Add the shared access policy to the share's policies. Note that each policy must have a unique name.
                permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
                await fileShare.SetPermissionsAsync(permissions);

                // Generate a SAS for a file in the share and associate this access policy with it.
                var rootDir = fileShare.GetRootDirectoryReference();
                var sampleDir = rootDir.GetDirectoryReference("CustomLogs");
                var file = sampleDir.GetFileReference("log1.txt");
                var sasToken = file.GetSharedAccessSignature(null, policyName);
                var fileSasUri = new Uri(file.StorageUri.PrimaryUri.ToString() + sasToken);

                // Create a new CloudFile object from the SAS, and write some text to the file.
                var fileSas = new CloudFile(fileSasUri);
                await fileSas.UploadTextAsync("This write operation is authorized via SAS.");
                Console.WriteLine(await fileSas.DownloadTextAsync());
            }
        }
    }
}