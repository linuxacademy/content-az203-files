using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.DataMovement;

namespace LinuxAcademy.AZ200.StorageSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            DML.blobUploadAsync().Wait();
            //FileUploadDmlAsync().Wait();
        }

    }
}
