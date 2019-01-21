using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage; // Namespace for Storage Client Library
using Microsoft.WindowsAzure.Storage.Table;

namespace LinuxAcademy.AZ200.StorageSamples
{
    public class CustomerEntity : TableEntity
    {
        public CustomerEntity()
        {
        }

        public CustomerEntity(string lastName, string firstName)
        {
            PartitionKey = lastName;
            RowKey = firstName;
        }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public override string ToString()
        {
            return $"CustomerEntity: {PartitionKey} {RowKey} {Email} {PhoneNumber}";
        }
    }
}