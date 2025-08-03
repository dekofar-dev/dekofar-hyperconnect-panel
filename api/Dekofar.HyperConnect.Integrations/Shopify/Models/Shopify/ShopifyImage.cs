using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Integrations.Shopify.Models.Shopify
{
    public class ShopifyImage
    {
        public long Id { get; set; }
        public string? Alt { get; set; }
        public int Position { get; set; }
        public long ProductId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Src { get; set; }
        public List<long>? VariantIds { get; set; }
    }
}
