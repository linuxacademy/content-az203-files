namespace linuxacademy.az203.paas.functions
{
    public class Order
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Description { get; set; }
    }
}