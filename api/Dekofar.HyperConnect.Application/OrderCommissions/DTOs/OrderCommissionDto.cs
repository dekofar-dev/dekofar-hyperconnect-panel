using System;

namespace Dekofar.HyperConnect.Application.OrderCommissions.DTOs
{
    public class OrderCommissionDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal CommissionRate { get; set; }
        public decimal EarnedAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
