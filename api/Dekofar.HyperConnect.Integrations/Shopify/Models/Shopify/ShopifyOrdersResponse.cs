using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Integrations.Shopify.Models.Shopify
{
    public class ShopifyOrdersResponse
    {
        [JsonProperty("orders")]
        public List<Order> Orders { get; set; }
    }
}
