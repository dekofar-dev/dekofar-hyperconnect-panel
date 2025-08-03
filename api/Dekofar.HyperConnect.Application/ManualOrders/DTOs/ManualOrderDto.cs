using Dekofar.HyperConnect.Domain.Entities;

namespace Dekofar.HyperConnect.Application.ManualOrders.DTOs
{
    public class ManualOrderDto
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
        public ManualOrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string? DiscountName { get; set; }
        public string? DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal BonusAmount { get; set; }

        public List<ManualOrderItemDto> Items { get; set; } = new();
    }

    public class ManualOrderItemDto
    {
        public Guid Id { get; set; }
        public string ProductId { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}

