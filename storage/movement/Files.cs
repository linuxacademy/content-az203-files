using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage; // Namespace for Storage Client Library
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Azure Blobs
using Microsoft.WindowsAzure.Storage.File; // Namespace for Azure Files

namespace LinuxAcademy.AZ200.StorageSamples
{
    public class FileSamples
    {
        private static string _shareName = "logs";

        public static CloudFileClient getCloudFileClient()
        {
            var storageAccount = Common.getCloudStorageAccount();
            var cloudFileClient = storageAccount.CreateCloudFileClient();
            return cloudFileClient;
        }

        public static CloudFileShare getFileShare()
        {
            var cfc = getCloudFileClient();
            var share = cfc.GetShareReference(_shareName);
            share.CreateIfNotExistsAsync();
            return share;
        }

        public static async Task writeFileContentAsync()
        {
            var share = getFileShare();
            var rootDir = share.GetRootDirectoryReference();

            var sampleDir = rootDir.GetDirectoryReference("Logs");
            if (!await sampleDir.ExistsAsync())
            {
                await sampleDir.CreateAsync();
            }
            // or use:
            // await sampleDir.CreateIfNotExistsAsync();

            var file = sampleDir.GetFileReference("log1.txt");
            if (!await file.ExistsAsync())
            {
                using (var stream = File.CreateText("log1.txt"))
                {
                    await stream.WriteLineAsync("Some log data");
                }
            }  

            Console.WriteLine(await file.DownloadTextAsync());
        }


        public static async Task setShareQuota()
        {
            var fileShare = getFileShare();

            // Check current usage stats for the share.
            // Note that the ShareStats object is part of the protocol layer for the File service.
            var stats = await fileShare.GetStatsAsync();
            Console.WriteLine("Current share usage: {0} GB", stats.Usage.ToString());

            // Specify the maximum size of the share, in GB.
            // This line sets the quota to be 10 GB greater than the current usage of the share.
            fileShare.Properties.Quota = 10 + stats.Usage;
            await fileShare.SetPropertiesAsync();

            // Now check the quota for the share. Call FetchAttributes() to populate the share's properties.
            await fileShare.FetchAttributesAsync();
            Console.WriteLine("Current share quota: {0} GB", fileShare.Properties.Quota);
        }

        public static async Task copyFileToFileAsync()
        {
            var fileShare = getFileShare();
            var root = fileShare.GetRootDirectoryReference();        
            var logsDir = root.GetDirectoryReference("Logs");
            var file1 = logsDir.GetFileReference("log1.txt");
            var file2 = logsDir.GetFileReference("log1-copy.txt");
            await file2.StartCopyAsync(file1); // this also works file to blob and vice-versa
            Console.WriteLine(await file2.DownloadTextAsync());        
        }
    }
}