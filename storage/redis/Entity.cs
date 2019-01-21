using System;
using StackExchange.Redis;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace LinuxAcademy.AZ200.Storage.RedisSample
{
    public class Entity : TableEntity
    {
        public string ID { get { return this.PartitionKey; } }
        public string Name { get { return this.RowKey; } }
        public string Description { get; set; }

        public Entity() {}
        public Entity(string id, string name, string description)
        {
            PartitionKey = id;
            RowKey = name;
            Description = description;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}