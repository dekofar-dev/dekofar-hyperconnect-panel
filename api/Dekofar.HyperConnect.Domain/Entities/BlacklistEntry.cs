using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public enum BlacklistType
    {
        IP,
        PhoneNumber,
        Email
    }

    public class BlacklistEntry
    {
        public int Id { get; set; }
        public BlacklistType Type { get; set; }
        public string Value { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
