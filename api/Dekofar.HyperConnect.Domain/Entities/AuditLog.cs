using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public string Action { get; set; } = default!;
        public string TargetType { get; set; } = default!;
        public Guid TargetId { get; set; }
        public string Description { get; set; } = default!;
        public Guid PerformedByUserId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
