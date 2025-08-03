using static System.Net.Mime.MediaTypeNames;

namespace Dekofar.HyperConnect.Integrations.Shopify.Models.Shopify
{
    public class ShopifyProduct
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string BodyHtml { get; set; }
        public string Vendor { get; set; }
        public string ProductType { get; set; }
        public string Handle { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<Variant> Variants { get; set; }
        public List<ShopifyImage> Images { get; set; }
    }
}
