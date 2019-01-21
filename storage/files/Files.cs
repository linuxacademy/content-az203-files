using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using Microsoft.WindowsAzure.Storage; 
using Microsoft.WindowsAzure.Storage.File;

namespace LinuxAcademy.AZ200.StorageSamples
{
    public class FileSamples
    {
        static string _connectionString = "";

        private static string _shareName = "logs";

        public static async Task runAsync()
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var cloudFileClient = storageAccount.CreateCloudFileClient();
            var share = cloudFileClient.GetShareReference(_shareName);
            await share.CreateIfNotExistsAsync();

            var rootDir = share.GetRootDirectoryReference();
            var sampleDir = rootDir.GetDirectoryReference("Logs");
            sampleDir.CreateIfNotExists();

            using (var stream = File.CreateText("log1.txt"))
            {
                await stream.WriteLineAsync("Some log data");
            }

            var file = sampleDir.GetFileReference("log1.txt");
            if (!await file.ExistsAsync())
            {
                await file.UploadFromFileAsync("log1.txt");
            }  

            var contents = await file.DownloadTextAsync();
            Console.WriteLine(contents);

            Console.WriteLine(file.StorageUri);
            await file.FetchAttributesAsync();
            Console.WriteLine(file.StorageUri);
            file.Metadata["docType"] = "text file";
            await file.SetMetadataAsync();
c
            file.DeleteIfExistsAsync();

            try {
                var newfile = sampleDir.GetFileReference("log1.txt");
                await newfile.UploadTextAsync("HI!");
            }
            catch (StorageException e) when (
                e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict && 
                e.RequestInformation.HttpStatusMessage == "SMBDeletePending") {
                // note exam may refer to e.RequestInformation.HttpStatusCode as response.StatusCode, and 
                //  e.RequestInformation.HttpStatusMessage as response.ReasonPhrase
            }

        }
    }
}