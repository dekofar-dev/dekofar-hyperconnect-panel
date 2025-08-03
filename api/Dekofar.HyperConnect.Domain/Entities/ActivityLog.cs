using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public class ActivityLog
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string? ActionData { get; set; }
        public string? IpAddress { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
