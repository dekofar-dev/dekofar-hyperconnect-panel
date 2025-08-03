using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Dekofar.HyperConnect.Application.ResponseTemplates.Commands
{
    public class CreateResponseTemplateCommand : IRequest<int>
    {
        [Required]
        public string Title { get; set; } = default!;
        [Required]
        public string Body { get; set; } = default!;
        public bool IsGlobal { get; set; }
        public string? ModuleScope { get; set; }
    }
}
