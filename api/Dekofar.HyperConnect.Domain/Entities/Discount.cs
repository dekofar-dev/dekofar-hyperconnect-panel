using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public class Discount
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public DiscountType Type { get; set; }
        public decimal Value { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public enum DiscountType
    {
        Percentage = 0,
        FixedAmount = 1
    }
}
