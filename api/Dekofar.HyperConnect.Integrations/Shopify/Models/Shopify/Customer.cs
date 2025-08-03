using Newtonsoft.Json;

public class Customer
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("first_name")]
    public string? FirstName { get; set; }

    [JsonProperty("last_name")]
    public string? LastName { get; set; }

    [JsonProperty("phone")]
    public string? Phone { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("orders_count")]
    public int OrdersCount { get; set; }
}
