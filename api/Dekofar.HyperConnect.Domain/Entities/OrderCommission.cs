using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public class OrderCommission
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal CommissionRate { get; set; }
        public decimal EarnedAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
