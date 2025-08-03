using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Integrations.Shopify.Models.Shopify.Dto
{
    public class ShopifyGraphQlPagedResult
    {
        public Data data { get; set; }
        public class Data
        {
            public Orders orders { get; set; }
        }

        public class Orders
        {
            public PageInfo pageInfo { get; set; }
            public List<Edge> edges { get; set; }
        }

        public class PageInfo
        {
            public bool hasNextPage { get; set; }
            public string endCursor { get; set; }
        }

        public class Edge
        {
            public OrderNode node { get; set; }
        }

        public class OrderNode
        {
            public string id { get; set; }
            public string name { get; set; }
            public string createdAt { get; set; }
            public TotalPriceSet totalPriceSet { get; set; }
            public Customer customer { get; set; }
        }

        public class TotalPriceSet
        {
            public ShopMoney shopMoney { get; set; }
        }

        public class ShopMoney
        {
            public string amount { get; set; }
            public string currencyCode { get; set; }
        }

        public class Customer
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string phone { get; set; }
        }
    }

}
