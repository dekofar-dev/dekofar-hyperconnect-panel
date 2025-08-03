using Dekofar.HyperConnect.Domain.Entities;
using Dekofar.HyperConnect.Integrations.Shopify.Models.Shopify;
using Dekofar.HyperConnect.Integrations.Shopify.Models.Shopify.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Integrations.Shopify.Interfaces
{
    public interface IShopifyService
    {
        /// <summary>
        /// Shopify bağlantısını test eder
        /// </summary>
        Task<string> TestConnectionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Sayfalı sipariş listesini getirir
        /// </summary>
        Task<PagedResult<Order>> GetOrdersPagedAsync(string? pageInfo = null, int limit = 10, CancellationToken ct = default);

        /// <summary>
        /// Sipariş ID'sine göre sipariş detayını getirir
        /// </summary>
        Task<Order?> GetOrderByIdAsync(long orderId, CancellationToken ct = default);

        /// <summary>
        /// Sipariş ID'sine göre sadeleştirilmiş sipariş detayını (ürün görselleri dahil) getirir
        /// </summary>
        Task<ShopifyOrderDetailDto?> GetOrderDetailWithImagesAsync(long orderId, CancellationToken ct = default);

        /// <summary>
        /// Tüm ürünleri getirir
        /// </summary>
        Task<List<ShopifyProduct>> GetAllProductsAsync(CancellationToken ct = default);

        /// <summary>
        /// Ürün ID'sine göre tekil ürün detayını getirir (görseller dahil)
        /// </summary>
        Task<ShopifyProduct?> GetProductByIdAsync(long productId, CancellationToken ct = default);

        /// <summary>
        /// Ürün adı veya handle'a göre arama yapar
        /// </summary>
        Task<List<ShopifyProduct>> SearchProductsAsync(string query, CancellationToken ct = default);

        /// <summary>
        /// Varyant ID'sine göre varyant bilgisini getirir
        /// </summary>
        Task<ShopifyVariant?> GetVariantByIdAsync(long variantId, CancellationToken ct = default);

        /// <summary>
        /// Bir ürüne ait tüm varyantları getirir
        /// </summary>
        Task<List<ShopifyVariant>> GetVariantsByProductIdAsync(long productId, CancellationToken ct = default);

        /// <summary>
        /// Belirtilen stok eşiğinin altında kalan ürünleri getirir
        /// </summary>
        /// <param name="threshold">Stok alt limiti (örn: 5)</param>
        Task<List<ShopifyProduct>> GetLowStockProductsAsync(int threshold, CancellationToken ct = default);

        /// <summary>
        /// Shopify ürününün etiketlerini (tags) günceller veya yenilerini ekler
        /// </summary>
        /// <param name="productId">Shopify ürün ID'si</param>
        /// <param name="tags">Virgül ile ayrılmış yeni etiketler</param>
        Task<bool> AddOrUpdateProductTagsAsync(long productId, string tags, CancellationToken ct = default);

        Task<List<Order>> SearchOrdersAsync(string query, CancellationToken ct = default);

        /// <summary>
        /// Search orders via Shopify GraphQL API.
        /// </summary>
        Task<List<Order>> GetOrdersBySearchQueryAsync(string query, CancellationToken ct = default);

        /// <summary>
        /// Update order tags.
        /// </summary>
        Task<bool> UpdateOrderTagsAsync(long orderId, string tags, CancellationToken ct = default);

        /// <summary>
        /// Update order note field.
        /// </summary>
        Task<bool> UpdateOrderNoteAsync(long orderId, string note, CancellationToken ct = default);

        /// <summary>
        /// Get customer details by ID.
        /// </summary>
        Task<Customer?> GetCustomerByIdAsync(long customerId, CancellationToken ct = default);

        /// <summary>
        /// Create a new order on Shopify.
        /// </summary>
        Task<Order?> CreateOrderAsync(Order order, CancellationToken ct = default);

        /// <summary>
        /// Create a fulfillment for an order.
        /// </summary>
        Task<string> CreateFulfillmentAsync(long orderId, FulfillmentCreateRequest request, CancellationToken ct = default);



        Task<PagedResult<Order>> GetOpenOrdersWithCursorAsync(string? pageInfo, int limit, CancellationToken ct);



    }
}
