using Dekofar.HyperConnect.Domain.Entities;
using Dekofar.HyperConnect.Integrations.Shopify.Interfaces;
using Dekofar.HyperConnect.Integrations.Shopify.Models;
using Dekofar.HyperConnect.Integrations.Shopify.Models.Shopify;
using Dekofar.HyperConnect.Integrations.Shopify.Models.Shopify.Dto;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Dekofar.HyperConnect.Integrations.Shopify.Services
{
    public class ShopifyService : IShopifyService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ShopifyService> _logger;
        private readonly string _baseUrl;
        private readonly string _accessToken;

        public ShopifyService(HttpClient httpClient, IConfiguration configuration, ILogger<ShopifyService> logger, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _memoryCache = memoryCache;

            _baseUrl = configuration["Shopify:BaseUrl"];
            _accessToken = configuration["Shopify:AccessToken"];

            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", _accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task<List<Order>> GetAllOrdersCachedAsync(CancellationToken ct = default)
        {
            const string cacheKey = "shopify_orders_cache";

            if (_memoryCache.TryGetValue(cacheKey, out List<Order> cachedOrders))
                return cachedOrders;

            var allOrders = new List<Order>();
            string? nextPageInfo = null;
            bool isFirstPage = true;
            int pageCounter = 0;
            int maxPages = 4; // 4 x 250 = 1000 sipariş

            do
            {
                string url = isFirstPage
                    ? "/admin/api/2024-04/orders.json?status=any&limit=250"
                    : $"/admin/api/2024-04/orders.json?limit=250&page_info={WebUtility.UrlEncode(nextPageInfo)}";

                isFirstPage = false;

                var response = await _httpClient.GetAsync(url, ct);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync(ct);
                    _logger.LogError("❌ Shopify API hatası: {StatusCode} - {Error}", response.StatusCode, error);
                    throw new Exception($"Shopify API hatası: {response.StatusCode} - {error}");
                }

                var content = await response.Content.ReadAsStringAsync(ct);
                var result = JsonConvert.DeserializeObject<OrdersResponse>(content);

                if (result?.Orders != null)
                    allOrders.AddRange(result.Orders);

                nextPageInfo = null;
                if (response.Headers.TryGetValues("Link", out var linkHeaders))
                {
                    var linkHeader = linkHeaders.FirstOrDefault();
                    var match = Regex.Match(linkHeader ?? "", @"<[^>]+page_info=([^&>]+)[^>]*>; rel=""next""");
                    if (match.Success)
                        nextPageInfo = match.Groups[1].Value;
                }

                pageCounter++;
                if (pageCounter >= maxPages) break;

            } while (!string.IsNullOrEmpty(nextPageInfo));

            _memoryCache.Set(cacheKey, allOrders, TimeSpan.FromMinutes(5)); // 5 dakikalık cache

            return allOrders;
        }

        /// <summary>
        /// Shopify mağazasıyla bağlantıyı test eder.
        /// /shop.json endpointine GET isteği atar ve mağaza bilgilerini döner.
        /// </summary>
        public async Task<string> TestConnectionAsync(CancellationToken cancellationToken = default)
        {
            // Mağaza bilgilerini almak için istek gönderilir
            var resp = await _httpClient.GetAsync("/admin/api/2024-04/shop.json", cancellationToken);

            // Başarısızsa exception fırlatır
            resp.EnsureSuccessStatusCode();

            // Yanıt içeriği okunur
            var content = await resp.Content.ReadAsStringAsync();

            // Loglama yapılır
            _logger.LogInformation("🔐 Shopify bağlantı başarılı. Yanıt: {Content}", content);

            return content;
        }

        /// <summary>
        /// Sayfalama destekli olarak açık (open) siparişleri çeker.
        /// Shopify'dan gelen Link header üzerinden next page bilgisi de ayrıştırılır.
        /// </summary>
        /// <param name="pageInfo">Sonraki sayfa bilgisi (Shopify'in verdiği page_info değeri)</param>
        /// <param name="limit">Sayfa başına kaç sipariş getirileceği</param>
        /// <param name="ct">İptal token'ı</param>
        /// <returns>PagedResult tipinde sipariş listesi ve varsa bir sonraki sayfa bilgisi</returns>
        public async Task<PagedResult<Order>> GetOrdersPagedAsync(string? pageInfo, int limit, CancellationToken ct)
        {
            var url = $"{_baseUrl}/admin/api/2023-04/orders.json?limit={limit}";

            // ⛔ Eğer page_info varsa, başka parametre geçme
            if (!string.IsNullOrWhiteSpace(pageInfo))
            {
                url += $"&page_info={WebUtility.UrlEncode(pageInfo)}";
            }
            else
            {
                // ✅ İlk istek ise, filtrelemeyi burada yap
                url += "&status=open&order=created_at+desc";
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", _accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Shopify API hatası: {(int)response.StatusCode} - {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var ordersWrapper = JsonConvert.DeserializeObject<ShopifyOrdersResponse>(content);

            var linkHeader = response.Headers.TryGetValues("Link", out var values) ? values.FirstOrDefault() : null;
            var nextPageInfo = ExtractNextPageInfoFromLinkHeader(linkHeader);

            return new PagedResult<Order>
            {
                Items = ordersWrapper?.Orders ?? new(),
                NextPageInfo = nextPageInfo
            };
        }

        private string? ExtractNextPageInfoFromLinkHeader(string? linkHeader)
        {
            if (string.IsNullOrWhiteSpace(linkHeader)) return null;

            // Örnek header:
            // <https://your-shop.myshopify.com/admin/api/2023-04/orders.json?page_info=xxx&limit=10>; rel="next"
            var match = Regex.Match(linkHeader, @"<[^>]*[?&]page_info=([^&>]*)[^>]*>; rel=""next""");

            return match.Success ? match.Groups[1].Value : null;
        }

        /// <summary>
        /// Belirli bir sipariş ID'sine göre Shopify sipariş detayını çeker.
        /// </summary>
        /// <param name="orderId">Shopify sipariş ID'si</param>
        /// <param name="ct">İptal token'ı</param>
        /// <returns>Sipariş bulunursa Order nesnesi, bulunamazsa null</returns>
        public async Task<Order?> GetOrderByIdAsync(long orderId, CancellationToken ct = default)
        {
            try
            {
                // API isteği için URL
                var url = $"/admin/api/2024-04/orders/{orderId}.json";

                // HTTP isteği gönder
                var response = await _httpClient.GetAsync(url, ct);
                response.EnsureSuccessStatusCode();

                // JSON içeriğini al
                var content = await response.Content.ReadAsStringAsync(ct);
                _logger.LogInformation("📄 Shopify sipariş detayı (ham JSON): {Content}", content);

                // JSON'u Order objesine deserialize et
                var parsed = JsonConvert.DeserializeObject<Dictionary<string, Order>>(content);

                // "order" alanı varsa döndür
                if (parsed != null && parsed.TryGetValue("order", out var order))
                    return order;

                _logger.LogWarning("⚠️ Sipariş detayı boş veya 'order' alanı eksik. ID: {OrderId}", orderId);
                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "❌ Shopify API hatası - OrderId: {OrderId}", orderId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Shopify sipariş detayı çekme hatası - ID: {OrderId}", orderId);
                throw new Exception("Shopify sipariş detayı çekilemedi.");
            }
        }
        /// <summary>
        /// Belirtilen ürün ve varyanta ait uygun görsel URL'sini getirir.
        /// </summary>
        /// <param name="productId">Ürün ID'si</param>
        /// <param name="variantId">Varyant ID'si (opsiyonel)</param>
        /// <param name="ct">İptal token'ı</param>
        /// <returns>Görsel URL'si veya null</returns>
        private async Task<string?> GetImageUrlFromProductAsync(long productId, long? variantId = null, CancellationToken ct = default)
        {
            // Ürünü Shopify API'den çek
            var productResp = await _httpClient.GetAsync($"/admin/api/2024-04/products/{productId}.json", ct);
            productResp.EnsureSuccessStatusCode();

            var content = await productResp.Content.ReadAsStringAsync(ct);
            dynamic productData = JsonConvert.DeserializeObject<dynamic>(content);
            var product = productData?.product;

            if (product == null || product.images == null)
                return null;

            long? imageId = null;

            // 1️⃣ Varyant ID varsa, buna karşılık gelen image_id bulunur
            if (variantId != null && product.variants != null)
            {
                foreach (var variant in product.variants)
                {
                    if ((long)variant.id == variantId)
                    {
                        imageId = variant.image_id;
                        break;
                    }
                }
            }

            // 2️⃣ image_id eşleşmesi varsa, karşılık gelen görsel döndürülür
            if (imageId != null)
            {
                foreach (var img in product.images)
                {
                    if ((long)img.id == imageId)
                        return (string)img.src;
                }
            }

            // 3️⃣ Yoksa ürünün ilk görseli döndürülür
            if (product.images.Count > 0)
                return (string)product.images[0].src;

            return null;
        }
        /// <summary>
        /// Belirtilen sipariş ID’sine ait detaylı sipariş bilgisini getirir.
        /// Her bir ürün kalemi için uygun ürün görseli de ilişkilendirilir.
        /// </summary>
        /// <param name="orderId">Shopify sipariş ID</param>
        /// <param name="ct">İptal token’ı</param>
        /// <returns>ShopifyOrderDetailDto – görseller dahil detaylı sipariş bilgisi</returns>
        public async Task<ShopifyOrderDetailDto?> GetOrderDetailWithImagesAsync(long orderId, CancellationToken ct = default)
        {
            // 🧾 Sipariş detayını getir
            var order = await GetOrderByIdAsync(orderId, ct);
            if (order == null) return null;

            var lineItems = new List<ShopifyOrderDetailDto.LineItemDto>();

            // 🛒 Sipariş içerisindeki ürünleri tek tek işle
            foreach (var item in order.LineItems)
            {
                string? imageUrl = null;

                try
                {
                    var productId = item.ProductId;
                    if (productId <= 0) continue;

                    // 📦 Ürün detaylarını çek
                    var productResp = await _httpClient.GetAsync($"/admin/api/2024-04/products/{productId}.json", ct);
                    productResp.EnsureSuccessStatusCode();

                    var content = await productResp.Content.ReadAsStringAsync(ct);
                    dynamic productData = JsonConvert.DeserializeObject<dynamic>(content);
                    var product = productData?.product;

                    if (product == null || product.images == null) continue;

                    long? imageId = null;

                    // 🔍 VARIANT_ID üzerinden image_id bul
                    if (item.VariantId.HasValue && product.variants != null)
                    {
                        foreach (var variant in product.variants)
                        {
                            if ((long)variant.id == item.VariantId.Value)
                            {
                                imageId = variant.image_id;
                                break;
                            }
                        }
                    }

                    // 🎯 image_id eşleşirse ilgili görseli kullan
                    if (imageId != null)
                    {
                        foreach (var img in product.images)
                        {
                            if ((long)img.id == imageId)
                            {
                                imageUrl = (string)img.src;
                                break;
                            }
                        }
                    }

                    // 🖼️ Eşleşme yoksa ilk görseli kullan
                    if (imageUrl == null && product.images.Count > 0)
                    {
                        imageUrl = (string)product.images[0].src;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"🖼️ Ürün görseli alınamadı (ProductId: {item.ProductId})");
                }

                // ➕ Görselli ürün kalemini DTO listesine ekle
                lineItems.Add(new ShopifyOrderDetailDto.LineItemDto
                {
                    Title = item.Title,
                    VariantTitle = item.VariantTitle,
                    Quantity = item.Quantity,
                    ImageUrl = imageUrl
                });
            }

            // ✅ Tüm sipariş verilerini DTO olarak hazırla ve dön
            return new ShopifyOrderDetailDto
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                CreatedAt = DateTime.Parse(order.CreatedAt),
                Currency = order.Currency,
                TotalPrice = order.TotalPrice,
                FinancialStatus = order.FinancialStatus,
                FulfillmentStatus = order.FulfillmentStatus,
                Note = order.Note,
                NoteAttributes = order.NoteAttributes,
                Tags = order.Tags,

                Customer = new CustomerDto
                {
                    Id = order.Customer?.Id ?? 0,
                    FirstName = order.Customer?.FirstName,
                    LastName = order.Customer?.LastName,
                    Phone = order.Customer?.Phone,
                    Email = order.Customer?.Email,
                    OrdersCount = order.Customer?.OrdersCount ?? 0
                },

                BillingAddress = new AddressDto
                {
                    FirstName = order.BillingAddress?.FirstName,
                    LastName = order.BillingAddress?.LastName,
                    Address1 = order.BillingAddress?.Address1,
                    Address2 = order.BillingAddress?.Address2,
                    City = order.BillingAddress?.City,
                    Province = order.BillingAddress?.Province,
                    Country = order.BillingAddress?.Country,
                    Zip = order.BillingAddress?.Zip,
                    Phone = order.BillingAddress?.Phone
                },

                LineItems = lineItems
            };
        }
        /// <summary>
        /// Shopify bağlantısının çalıştığını test etmek için basit bir GET isteği yapar.
        /// </summary>
        Task<string> IShopifyService.TestConnectionAsync(CancellationToken cancellationToken)
        {
            return TestConnectionAsync(cancellationToken);
        }
        /// <summary>
        /// Shopify'dan sayfalı sipariş listesini getirir (open statüsündeki siparişler).
        /// </summary>
        Task<PagedResult<Order>> IShopifyService.GetOrdersPagedAsync(string? pageInfo, int limit, CancellationToken ct)
        {
            return GetOrdersPagedAsync(pageInfo, limit, ct);
        }
        /// <summary>
        /// Belirtilen ID’ye sahip Shopify siparişini getirir.
        /// </summary>
        Task<Order?> IShopifyService.GetOrderByIdAsync(long orderId, CancellationToken ct)
        {
            return GetOrderByIdAsync(orderId, ct);
        }
        /// <summary>
        /// Sipariş ID’sine göre sadeleştirilmiş sipariş detayını (ürün görselleri dahil) getirir.
        /// </summary>
        Task<ShopifyOrderDetailDto?> IShopifyService.GetOrderDetailWithImagesAsync(long orderId, CancellationToken ct)
        {
            return GetOrderDetailWithImagesAsync(orderId, ct);
        }
        /// <summary>
        /// Shopify mağazasındaki tüm ürünleri getirir.
        /// </summary>
        public async Task<List<ShopifyProduct>> GetAllProductsAsync(CancellationToken ct = default)
        {
            var response = await _httpClient.GetAsync("/admin/api/2024-04/products.json", ct);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(ct);
            var parsed = JsonConvert.DeserializeObject<Dictionary<string, List<ShopifyProduct>>>(content);

            if (parsed != null && parsed.TryGetValue("products", out var products))
                return products;

            return new List<ShopifyProduct>();
        }
        /// <summary>
        /// Belirli bir ürün ID'sine göre Shopify ürününü getirir.
        /// </summary>
        public async Task<ShopifyProduct?> GetProductByIdAsync(long productId, CancellationToken ct = default)
        {
            var response = await _httpClient.GetAsync($"/admin/api/2024-04/products/{productId}.json", ct);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(ct);
            var parsed = JsonConvert.DeserializeObject<Dictionary<string, ShopifyProduct>>(content);

            if (parsed != null && parsed.TryGetValue("product", out var product))
                return product;

            return null;
        }
        /// <summary>
        /// Ürün başlığına göre Shopify ürünlerinde arama yapar.
        /// </summary>
        public async Task<List<ShopifyProduct>> SearchProductsAsync(string query, CancellationToken ct = default)
        {
            // title parametresi ile başlıkta geçen ürünleri filtrele
            var url = $"/admin/api/2024-04/products.json?title={WebUtility.UrlEncode(query)}&limit=50";

            var response = await _httpClient.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(ct);
            var parsed = JsonConvert.DeserializeObject<Dictionary<string, List<ShopifyProduct>>>(content);

            if (parsed != null && parsed.TryGetValue("products", out var products))
                return products;

            return new List<ShopifyProduct>();
        }
        /// <summary>
        /// Variant ID’sine göre Shopify ürün varyantını getirir.
        /// </summary>
        public async Task<ShopifyVariant?> GetVariantByIdAsync(long variantId, CancellationToken ct = default)
        {
            var url = $"/admin/api/2024-04/variants/{variantId}.json";

            var response = await _httpClient.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(ct);
            var parsed = JsonConvert.DeserializeObject<Dictionary<string, ShopifyVariant>>(content);

            if (parsed != null && parsed.TryGetValue("variant", out var variant))
                return variant;

            return null;
        }
        /// <summary>
        /// Belirtilen ürün ID'sine ait tüm varyantları getirir.
        /// </summary>
        public async Task<List<ShopifyVariant>> GetVariantsByProductIdAsync(long productId, CancellationToken ct = default)
        {
            var url = $"/admin/api/2024-04/products/{productId}.json";

            var response = await _httpClient.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(ct);
            dynamic productData = JsonConvert.DeserializeObject<dynamic>(content);

            var variants = new List<ShopifyVariant>();

            if (productData?.product?.variants != null)
            {
                foreach (var variant in productData.product.variants)
                {
                    var variantJson = JsonConvert.SerializeObject(variant);
                    var parsedVariant = JsonConvert.DeserializeObject<ShopifyVariant>(variantJson);
                    if (parsedVariant != null)
                        variants.Add(parsedVariant);
                }
            }

            return variants;
        }
        /// <summary>
        /// Stok adedi belirtilen eşik değerinden düşük olan ürünleri getirir (tüm varyantlar taranır).
        /// </summary>
        public async Task<List<ShopifyProduct>> GetLowStockProductsAsync(int threshold, CancellationToken ct = default)
        {
            var lowStockProducts = new List<ShopifyProduct>();

            var response = await _httpClient.GetAsync("/admin/api/2024-04/products.json?limit=250", ct);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(ct);
            var parsed = JsonConvert.DeserializeObject<Dictionary<string, List<ShopifyProduct>>>(content);

            if (parsed != null && parsed.TryGetValue("products", out var allProducts))
            {
                foreach (var product in allProducts)
                {
                    if (product.Variants != null && product.Variants.Any(v => v.InventoryQuantity < threshold))
                    {
                        lowStockProducts.Add(product);
                    }
                }
            }

            return lowStockProducts;
        }
        /// <summary>
        /// Belirtilen ürünün etiketlerini günceller veya yeni etiket ekler.
        /// </summary>
        /// <param name="productId">Shopify ürün ID'si</param>
        /// <param name="tags">Virgülle ayrılmış etiket listesi (örneğin: "stok_dustu,kritik")</param>
        /// <param name="ct">İptal tokeni</param>
        /// <returns>true: başarıyla güncellendi | false: hata oluştu</returns>
        public async Task<bool> AddOrUpdateProductTagsAsync(long productId, string tags, CancellationToken ct = default)
        {
            var requestBody = new
            {
                product = new
                {
                    id = productId,
                    tags = tags
                }
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/admin/api/2024-04/products/{productId}.json", content, ct);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"🏷️ Ürün etiketleri güncellendi (ID: {productId}, Tags: {tags})");
                return true;
            }

            _logger.LogWarning($"⚠️ Ürün etiketleri güncellenemedi (ID: {productId}) - StatusCode: {response.StatusCode}");
            return false;
        }

        /// <summary>
        /// Update order tags field.
        /// </summary>
        public async Task<bool> UpdateOrderTagsAsync(long orderId, string tags, CancellationToken ct = default)
        {
            var body = new { order = new { id = orderId, tags } };
            var json = JsonConvert.SerializeObject(body);
            var resp = await _httpClient.PutAsync($"/admin/api/2024-04/orders/{orderId}.json", new StringContent(json, Encoding.UTF8, "application/json"), ct);
            return resp.IsSuccessStatusCode;
        }

        /// <summary>
        /// Update order note field.
        /// </summary>
        public async Task<bool> UpdateOrderNoteAsync(long orderId, string note, CancellationToken ct = default)
        {
            var body = new { order = new { id = orderId, note } };
            var json = JsonConvert.SerializeObject(body);
            var resp = await _httpClient.PutAsync($"/admin/api/2024-04/orders/{orderId}.json", new StringContent(json, Encoding.UTF8, "application/json"), ct);
            return resp.IsSuccessStatusCode;
        }

        /// <summary>
        /// Retrieve customer information by ID.
        /// </summary>
        public async Task<Customer?> GetCustomerByIdAsync(long customerId, CancellationToken ct = default)
        {
            var resp = await _httpClient.GetAsync($"/admin/api/2024-04/customers/{customerId}.json", ct);
            if (!resp.IsSuccessStatusCode) return null;
            var content = await resp.Content.ReadAsStringAsync(ct);
            var parsed = JsonConvert.DeserializeObject<Dictionary<string, Customer>>(content);
            return parsed != null && parsed.TryGetValue("customer", out var c) ? c : null;
        }

        /// <summary>
        /// Create a new order on Shopify.
        /// </summary>
        public async Task<Order?> CreateOrderAsync(Order order, CancellationToken ct = default)
        {
            var body = new { order };
            var json = JsonConvert.SerializeObject(body);
            var resp = await _httpClient.PostAsync("/admin/api/2024-04/orders.json", new StringContent(json, Encoding.UTF8, "application/json"), ct);
            if (!resp.IsSuccessStatusCode) return null;
            var content = await resp.Content.ReadAsStringAsync(ct);
            var parsed = JsonConvert.DeserializeObject<Dictionary<string, Order>>(content);
            return parsed != null && parsed.TryGetValue("order", out var o) ? o : null;
        }

        /// <summary>
        /// Create fulfillment for an order.
        /// </summary>
        public async Task<string> CreateFulfillmentAsync(long orderId, FulfillmentCreateRequest request, CancellationToken ct = default)
        {
            var body = new { fulfillment = new { location_id = request.LocationId, tracking_number = request.TrackingNumber, tracking_company = request.TrackingCompany } };
            var json = JsonConvert.SerializeObject(body);
            var resp = await _httpClient.PostAsync($"/admin/api/2024-04/orders/{orderId}/fulfillments.json", new StringContent(json, Encoding.UTF8, "application/json"), ct);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStringAsync(ct);
        }

        /// <summary>
        /// Search orders via GraphQL and cache result.
        /// </summary>
        public async Task<List<Order>> GetOrdersBySearchQueryAsync(string query, CancellationToken ct = default)
        {
            var cacheKey = $"shopify_order_search_{query}";
            if (_memoryCache.TryGetValue(cacheKey, out List<Order> cached))
                return cached;

            var gql = $"{{ orders(first: 250, query: \"{query}\") {{ edges {{ node {{ id name createdAt totalPriceSet {{ shopMoney {{ amount currencyCode }} }} customer {{ firstName lastName phone }} }} }} }} }}";
            var payload = new StringContent(JsonConvert.SerializeObject(new { query = gql }), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/admin/api/2024-04/graphql.json", payload, ct);
            resp.EnsureSuccessStatusCode();
            var content = await resp.Content.ReadAsStringAsync(ct);
            var result = JsonConvert.DeserializeObject<ShopifyGraphQlPagedResult>(content);

            var orders = new List<Order>();
            if (result?.data?.orders?.edges != null)
            {
                foreach (var edge in result.data.orders.edges)
                {
                    var node = edge.node;
                    var idPart = node.id?.Split('/')?.LastOrDefault();
                    long.TryParse(idPart, out var id);
                    orders.Add(new Order
                    {
                        Id = id,
                        OrderNumber = node.name,
                        CreatedAt = node.createdAt,
                        TotalPrice = node.totalPriceSet?.shopMoney?.amount,
                        Currency = node.totalPriceSet?.shopMoney?.currencyCode,
                        Customer = node.customer == null ? null : new Customer
                        {
                            FirstName = node.customer.firstName,
                            LastName = node.customer.lastName,
                            Phone = node.customer.phone
                        }
                    });
                }
            }

            _memoryCache.Set(cacheKey, orders, TimeSpan.FromMinutes(5));
            return orders;
        }
        public async Task<List<Order>> SearchOrdersAsync(string query, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return new List<Order>();

            // Prefer GraphQL search which fetches first 250 matching orders.
            var result = await GetOrdersBySearchQueryAsync(query, ct);
            return result;
        }



        public async Task<PagedResult<Order>> GetOpenOrdersWithCursorAsync(string? pageInfo, int limit, CancellationToken ct)
        {
            var url = $"{_baseUrl}/admin/api/2023-04/orders.json?limit={limit}";

            if (!string.IsNullOrWhiteSpace(pageInfo))
            {
                // Sayfalamada status filtresi KULLANILMAZ!
                url += $"&page_info={WebUtility.UrlEncode(pageInfo)}";
            }
            else
            {
                // İlk sayfa sorgusu — filtre sadece burada geçerli
                url += "&status=open&order=created_at+desc";
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", _accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"Shopify API hatası: {(int)response.StatusCode} - {err}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var ordersWrapper = JsonConvert.DeserializeObject<ShopifyOrdersResponse>(content);

            var linkHeader = response.Headers.TryGetValues("Link", out var values) ? values.FirstOrDefault() : null;
            var nextPageInfo = ExtractNextPageInfoFromLinkHeader(linkHeader);

            return new PagedResult<Order>
            {
                Items = ordersWrapper?.Orders ?? new(),
                NextPageInfo = nextPageInfo
            };
        }




    }
}