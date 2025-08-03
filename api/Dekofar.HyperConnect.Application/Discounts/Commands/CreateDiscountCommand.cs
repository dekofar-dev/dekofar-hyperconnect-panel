using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dekofar.HyperConnect.Application.Discounts.Commands
{
    public class CreateDiscountCommand : IRequest<Guid>
    {
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public DiscountType Type { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Value { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
