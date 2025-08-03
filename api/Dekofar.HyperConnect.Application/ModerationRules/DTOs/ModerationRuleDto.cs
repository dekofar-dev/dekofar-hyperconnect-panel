using Dekofar.HyperConnect.Domain.Entities;

namespace Dekofar.HyperConnect.Application.ModerationRules.DTOs
{
    public class ModerationRuleDto
    {
        public int Id { get; set; }
        public string Pattern { get; set; } = default!;
        public ModerationSeverity Severity { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
