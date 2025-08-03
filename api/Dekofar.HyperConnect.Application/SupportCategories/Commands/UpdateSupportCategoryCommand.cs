using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dekofar.HyperConnect.Application.SupportCategories.Commands
{
    public class UpdateSupportCategoryCommand : IRequest<Unit>
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}
