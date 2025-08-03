using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Domain.Entities.Orders
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? ColorHex { get; set; } // örnek: #6a11cb
        public string? Description { get; set; }

        public ICollection<OrderTag> OrderTags { get; set; } = new List<OrderTag>();
    }
}
