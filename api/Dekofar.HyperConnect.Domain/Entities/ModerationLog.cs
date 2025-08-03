using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public class ModerationLog
    {
        public int Id { get; set; }
        public string Content { get; set; } = default!;
        public int RuleId { get; set; }
        public ModerationSeverity Severity { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
