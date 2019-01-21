using System;
using System.IO;
using Microsoft.WindowsAzure.Storage; 
using Microsoft.WindowsAzure.Storage.Blob; 
using Microsoft.WindowsAzure.Storage.File; 

namespace LinuxAcademy.AZ200.StorageSamples
{
    public class Common
    {
        public static string getConnectionString()
        {
            var acctName = "laaz200stg"; //Environment.GetEnvironmentVariable("LAAZ200STGACCTNAME");
            var acctKey = ""; //Environment.GetEnvironmentVariable("LAAZ200STGACCTKEY");
            var connectionString = 
                $"";

            return connectionString;
        }

        public static CloudStorageAccount getCloudStorageAccount()
        {
            var connectionString = getConnectionString();
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            return storageAccount;
        }
    }
}