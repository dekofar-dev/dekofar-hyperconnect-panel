using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Domain.Entities.Orders
{
    public class OrderTag
    {
        public int Id { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; } = default!;

        public long? ShopifyOrderId { get; set; } // sadece ID tutulur, navigation yok
        public int? ManualOrderId { get; set; }   // ileride entegre edilecek

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedByUserId { get; set; }
    }

}
