using System;
using System.Collections.Generic;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public class ManualOrder
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = default!;
        public string CustomerSurname { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string? Email { get; set; }
        public string Address { get; set; } = default!;
        public string City { get; set; } = default!;
        public string District { get; set; } = default!;
        public string PaymentType { get; set; } = default!;
        public string? OrderNote { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ManualOrderStatus Status { get; set; } = ManualOrderStatus.Pending;
        public decimal TotalAmount { get; set; }
        public string? DiscountName { get; set; }
        public string? DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal BonusAmount { get; set; }

        public ICollection<ManualOrderItem> Items { get; set; } = new List<ManualOrderItem>();
    }

    public enum ManualOrderStatus
    {
        Pending = 0,
        Confirmed = 1,
        Delivered = 2,
        Cancelled = 3
    }
}

