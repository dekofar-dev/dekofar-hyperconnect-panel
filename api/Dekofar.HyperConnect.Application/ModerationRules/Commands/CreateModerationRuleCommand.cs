using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Dekofar.HyperConnect.Application.ModerationRules.Commands
{
    public class CreateModerationRuleCommand : IRequest<int>
    {
        [Required]
        public string Pattern { get; set; } = default!;
        public ModerationSeverity Severity { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
