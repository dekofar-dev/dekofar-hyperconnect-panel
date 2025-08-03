using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public class Commission
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = default!;

        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid? OrderId { get; set; }
        public Order? Order { get; set; }

        public string? Description { get; set; }
    }
}
