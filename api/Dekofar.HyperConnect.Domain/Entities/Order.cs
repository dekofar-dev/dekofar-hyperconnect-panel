using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }

        public Guid? SellerId { get; set; }
        public ApplicationUser? Seller { get; set; }

        public Guid? CustomerId { get; set; }
        public ApplicationUser? Customer { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Optional: integration with other systems could go here
    }

    public enum OrderStatus
    {
        Pending = 0,
        Shipped = 1,
        Cancelled = 2
    }
}
