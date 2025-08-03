namespace Dekofar.HyperConnect.Integrations.Shopify.Models.Shopify.Dto
{
    /// <summary>
    /// Request body for updating order tags.
    /// </summary>
    public class UpdateOrderTagsRequest
    {
        public long OrderId { get; set; }
        public string Tags { get; set; } = string.Empty;
    }
}
