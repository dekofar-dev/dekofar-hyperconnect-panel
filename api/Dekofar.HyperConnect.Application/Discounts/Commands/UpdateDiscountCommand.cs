using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dekofar.HyperConnect.Application.Discounts.Commands
{
    public class UpdateDiscountCommand : IRequest<Unit>
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public DiscountType Type { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Value { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
