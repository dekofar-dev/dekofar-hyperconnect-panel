using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Integrations.Shopify.Models.Shopify
{
    public class ShopifyVariant
    {
        public long Id { get; set; }                          // Varyant ID'si
        public long ProductId { get; set; }                   // Ait olduğu ürünün ID'si
        public string? Title { get; set; }                    // Varyant başlığı (ör: "Mavi", "XL")
        public string? Sku { get; set; }                      // Stok kodu
        public string? Barcode { get; set; }                  // Barkod (varsa)
        public string? Price { get; set; }                    // Fiyat
        public string? CompareAtPrice { get; set; }           // İndirim öncesi fiyat (varsa)
        public int InventoryQuantity { get; set; }            // Mevcut stok adedi
        public string? InventoryManagement { get; set; }      // Envanter yönetim tipi ("shopify" olabilir)
        public string? InventoryPolicy { get; set; }          // Stok politikası (örn: "deny")
        public string? FulfillmentService { get; set; }       // Kargo hizmet tipi (örn: "manual")
        public double Weight { get; set; }                    // Ağırlık
        public string? WeightUnit { get; set; }               // Ağırlık birimi (örn: "kg")
        public long? ImageId { get; set; }                    // Bağlı olduğu görselin ID'si (varsa)
        public string? Option1 { get; set; }                  // Varyant seçeneği 1 (örn: Renk)
        public string? Option2 { get; set; }                  // Varyant seçeneği 2 (örn: Beden)
        public string? Option3 { get; set; }                  // Varyant seçeneği 3 (varsa)
        public string? CreatedAt { get; set; }                // Oluşturulma zamanı
        public string? UpdatedAt { get; set; }                // Güncellenme zamanı
    }
}
