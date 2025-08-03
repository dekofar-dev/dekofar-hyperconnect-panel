using Dekofar.HyperConnect.Integrations.Shopify.Models.Shopify;
using Dekofar.HyperConnect.Integrations.Shopify.Models.Shopify.Dto;

public class ShopifyOrderDetailDto
{

    public long OrderId { get; set; }
    public string OrderNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TotalPrice { get; set; }
    public string Currency { get; set; }
    public string FinancialStatus { get; set; }
    public string FulfillmentStatus { get; set; }
    public string? Note { get; set; }
    public string? Tags { get; set; }
    public List<NoteAttribute>? NoteAttributes { get; set; }

    public CustomerDto Customer { get; set; }
    public AddressDto BillingAddress { get; set; }

    public List<LineItemDto> LineItems { get; set; }

    public class LineItemDto
    {
        public string Title { get; set; }
        public string? VariantTitle { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }  // 🔥 Görsel burada taşınıyor
    }
}
