using Newtonsoft.Json;

public class ProductDto
{
    [JsonProperty("image")]
    public ProductImageDto? Image { get; set; }

    [JsonProperty("images")]
    public List<ProductImageDto>? Images { get; set; }
}

public class ProductImageDto
{
    [JsonProperty("src")]
    public string? Src { get; set; }
}
