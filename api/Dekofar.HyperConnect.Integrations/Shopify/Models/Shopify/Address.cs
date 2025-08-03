using Newtonsoft.Json;

namespace Dekofar.HyperConnect.Integrations.Shopify.Models.Shopify
{
    public class Address
    {
        [JsonProperty("first_name")]
        public string? FirstName { get; set; }

        [JsonProperty("last_name")]
        public string? LastName { get; set; }

        [JsonProperty("address1")]
        public string? Address1 { get; set; }

        [JsonProperty("address2")]
        public string? Address2 { get; set; }

        [JsonProperty("city")]
        public string? City { get; set; }

        [JsonProperty("province")]
        public string? Province { get; set; }

        [JsonProperty("country")]
        public string? Country { get; set; }

        [JsonProperty("zip")]
        public string? Zip { get; set; }

        [JsonProperty("phone")]
        public string? Phone { get; set; }
    }
}
