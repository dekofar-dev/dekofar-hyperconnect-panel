using Dekofar.HyperConnect.Integrations.Shopify.Models.Shopify;
using Newtonsoft.Json;
using System.Net;

public class Order
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("order_number")]
    public string OrderNumber { get; set; }

    [JsonProperty("created_at")]
    public string CreatedAt { get; set; }

    [JsonProperty("total_price")]
    public string TotalPrice { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; }

    [JsonProperty("financial_status")]
    public string FinancialStatus { get; set; }

    [JsonProperty("fulfillment_status")]
    public string FulfillmentStatus { get; set; }

    [JsonProperty("note")]
    public string? Note { get; set; }

    [JsonProperty("tags")]
    public string? Tags { get; set; }

    [JsonProperty("customer")]
    public Customer? Customer { get; set; }

    [JsonProperty("billing_address")]
    public Address? BillingAddress { get; set; }

    [JsonProperty("note_attributes")]
    public List<NoteAttribute>? NoteAttributes { get; set; }


    [JsonProperty("line_items")]
    public List<LineItem> LineItems { get; set; } = new(); // 👈 Ürünler
    public string Name { get; internal set; }
}
