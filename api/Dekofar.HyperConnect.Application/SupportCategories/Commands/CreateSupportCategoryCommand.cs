using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dekofar.HyperConnect.Application.SupportCategories.Commands
{
    public class CreateSupportCategoryCommand : IRequest<Guid>
    {
        [Required]
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}
