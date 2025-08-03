using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Dekofar.HyperConnect.Application.ResponseTemplates.Commands
{
    public class UpdateResponseTemplateCommand : IRequest
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = default!;
        [Required]
        public string Body { get; set; } = default!;
        public bool IsGlobal { get; set; }
        public string? ModuleScope { get; set; }
    }
}
