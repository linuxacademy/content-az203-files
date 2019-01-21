using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.DataMovement;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;

namespace LinuxAcademy.AZ200.StorageSamples
{
    public class DML
    {
        private const string _containerName = "samplecontainer";
        private const string _shareName = "sampleshare";


        private static CloudStorageAccount _storageAccount;
        private static CloudBlobClient _blobClient;
        private static CloudFileClient _fileClient;

        private static CloudStorageAccount getStorageAccount()
        {
            if (_storageAccount == null)
            {
                var connectionString = Common.getConnectionString();
                _storageAccount = CloudStorageAccount.Parse(connectionString);
            }

            return _storageAccount;
        }

        public static async Task<CloudBlob> getCloudBlobAsync(string containerName, string blobName, BlobType blobType)
        {
            var client = getCloudBlobClient();
            var container = client.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            CloudBlob cloudBlob;
            switch (blobType)
            {
                case BlobType.AppendBlob:
                    cloudBlob = container.GetAppendBlobReference(blobName);
                    break;
                case BlobType.BlockBlob:
                    cloudBlob = container.GetBlockBlobReference(blobName);
                    break;
                case BlobType.PageBlob:
                    cloudBlob = container.GetPageBlobReference(blobName);
                    break;
                case BlobType.Unspecified:
                default:
                    throw new ArgumentException(string.Format("Invalid blob type {0}", blobType.ToString()), "blobType");
            }

            return cloudBlob;
        }

        private static CloudBlobClient getCloudBlobClient()
        {
            if (_blobClient == null)
            {
                _blobClient = getStorageAccount().CreateCloudBlobClient();
            }

            return _blobClient;
        }

        private static CloudFileClient getCloudFileClient()
        {
            if (_fileClient == null)
            {
                _fileClient = getStorageAccount().CreateCloudFileClient();
            }
            
            return _fileClient;
        }

        public static async Task<CloudFile> getCloudFileAsync(string shareName, string fileName)
        {
            var client = getCloudFileClient();
            var share = client.GetShareReference(shareName);
            await share.CreateIfNotExistsAsync();

            var rootDirectory = share.GetRootDirectoryReference();
            return rootDirectory.GetFileReference(fileName);
        }

        public static async Task blobUploadAsync()
        {
            // When transfer large file to block blob, set TransferManager.Configurations.BlockSize to specify the size of the blocks.
            // It must be between 4MB and 100MB and be multiple of 4MB. Default value is 4MB. 
            //
            // Currently, the max block count of a block blob is limited to 50000.
            // When transfering a big file and the BlockSize provided is smaller than the minimum value - (size/50000),
            // it'll be reset to a value which is greater than the minimum value and multiple of 4MB for this file.
            TransferManager.Configurations.BlockSize = 4 * 1024 * 1024; //4MB

            var sourceFileName = "azure.png";
            var destinationBlobName = "azure_blockblob.png";

            // Create the destination CloudBlob instance
            var destinationBlob = await getCloudBlobAsync(_containerName, destinationBlobName, BlobType.BlockBlob);

            // Use UploadOptions to set ContentType of destination CloudBlob
            var options = new UploadOptions();

            var context = new SingleTransferContext();
            context.SetAttributesCallbackAsync = async (destination) =>
            {
                var destBlob = destination as CloudBlob;
                destBlob.Properties.ContentType = "image/png";
                await Task.CompletedTask;
            };

            context.ShouldOverwriteCallbackAsync = TransferContext.ForceOverwrite;

            // Start the upload
            await TransferManager.UploadAsync(sourceFileName, destinationBlob, options, context);
            Console.WriteLine("File {0} is uploaded to {1} successfully.", sourceFileName, destinationBlob.Uri.ToString());
        }

        public static async Task fileUploadAsync()
        {
            var sourceFileName = "azure.png";
            var destinationFileName = "azure_cloudfile.png";

            // Create the destination CloudFile instance
            var destinationFile = await getCloudFileAsync(_shareName, destinationFileName);

            // Start the upload
            await TransferManager.UploadAsync(sourceFileName, destinationFile);
            Console.WriteLine("File {0} is uploaded to {1} successfully.", sourceFileName, destinationFile.Uri.ToString());
         }
    }
}