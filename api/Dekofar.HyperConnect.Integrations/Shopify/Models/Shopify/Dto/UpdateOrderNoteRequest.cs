namespace Dekofar.HyperConnect.Integrations.Shopify.Models.Shopify.Dto
{
    /// <summary>
    /// Request body for updating order note.
    /// </summary>
    public class UpdateOrderNoteRequest
    {
        public long OrderId { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
