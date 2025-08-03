using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Integrations.Shopify.Models
{
    public class ShopifyPagedResponse
    {
        [JsonProperty("orders")]
        public List<Order> Orders { get; set; } = new();
    }


}
