using System;

namespace Dekofar.HyperConnect.Application.Discounts.DTOs
{
    public class DiscountDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        public decimal Value { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
