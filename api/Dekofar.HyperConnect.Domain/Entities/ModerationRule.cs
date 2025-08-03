using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public class ModerationRule
    {
        public int Id { get; set; }
        public string Pattern { get; set; } = default!;
        public ModerationSeverity Severity { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public enum ModerationSeverity
    {
        Warning,
        Block
    }
}
