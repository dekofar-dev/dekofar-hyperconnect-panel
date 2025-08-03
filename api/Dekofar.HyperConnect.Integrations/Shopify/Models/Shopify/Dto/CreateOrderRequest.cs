using Dekofar.HyperConnect.Integrations.Shopify.Models.Shopify;
namespace Dekofar.HyperConnect.Integrations.Shopify.Models.Shopify.Dto
{
    /// <summary>
    /// Wrapper for creating a new order via Shopify API.
    /// </summary>
    public class CreateOrderRequest
    {
        public Order Order { get; set; } = new();
    }
}
