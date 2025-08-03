using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dekofar.HyperConnect.Application.SupportCategories.Commands
{
    public class AssignRolesToSupportCategoryCommand : IRequest<Unit>
    {
        [Required]
        public Guid SupportCategoryId { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
