namespace Dekofar.HyperConnect.Integrations.Shopify.Models.Shopify.Dto
{
    /// <summary>
    /// Request body for creating a fulfillment.
    /// </summary>
    public class FulfillmentCreateRequest
    {
        public long OrderId { get; set; }
        public long? LocationId { get; set; }
        public string? TrackingNumber { get; set; }
        public string? TrackingCompany { get; set; }
    }
}
