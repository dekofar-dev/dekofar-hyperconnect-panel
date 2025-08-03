using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dekofar.HyperConnect.Application.ManualOrders.Commands
{
    public class CreateManualOrderCommand : IRequest<Guid>
    {
        [Required]
        public string CustomerName { get; set; } = default!;
        [Required]
        public string CustomerSurname { get; set; } = default!;
        [Required]
        public string Phone { get; set; } = default!;
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string Address { get; set; } = default!;
        [Required]
        public string City { get; set; } = default!;
        [Required]
        public string District { get; set; } = default!;
        [Required]
        public string PaymentType { get; set; } = default!;
        public string? OrderNote { get; set; }
        public Guid? DiscountId { get; set; }
        public string? DiscountName { get; set; }

        [MinLength(1)]
        public List<CreateManualOrderItemDto> Items { get; set; } = new();
    }

    public class CreateManualOrderItemDto
    {
        [Required]
        public string ProductId { get; set; } = default!;
        [Required]
        public string ProductName { get; set; } = default!;
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}

