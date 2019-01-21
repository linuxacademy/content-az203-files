using Newtonsoft.Json;

namespace LinuxAcademy.AZ200.CosmosDbSample
{
    public class Customer
    {
        public string id { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class OrderItem
    {
        public string OrderItemId  { get; set; }
        public string Description { get; set;}
        public int Quantity { get; set;}
        public double Price { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Address
    {
        public string City { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Order
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public Customer Customer { get; set; }
        public OrderItem[] OrderItems { get; set; }
        public Address ShipTo { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}