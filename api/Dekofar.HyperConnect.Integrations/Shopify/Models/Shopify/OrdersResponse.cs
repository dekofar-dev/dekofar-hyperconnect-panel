using Newtonsoft.Json;

public class OrdersResponse
{
    [JsonProperty("orders")]
    public List<Order> Orders { get; set; } = new();
}
