using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Integrations.Shopify.Models.Shopify
{
    public class Variant
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Price { get; set; }
        public string Sku { get; set; }
        public long ProductId { get; set; }
        public int Position { get; set; }
        public string Option1 { get; set; }
        public long? ImageId { get; set; }
        public int InventoryQuantity { get; internal set; }
    }

}
