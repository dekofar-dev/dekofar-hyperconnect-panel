using Newtonsoft.Json;

public class LineItem
{
    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("variant_title")]
    public string? VariantTitle { get; set; }

    [JsonProperty("quantity")]
    public int Quantity { get; set; }

    [JsonProperty("product_id")]
    public long ProductId { get; set; }

    [JsonProperty("variant_id")]
    public long? VariantId { get; set; }

    [JsonProperty("image_id")]
    public long? ImageId { get; set; }
}
