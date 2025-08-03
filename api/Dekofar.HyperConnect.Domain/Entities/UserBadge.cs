using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public class UserBadge
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Badge { get; set; } = string.Empty;
        public DateTime AwardedAt { get; set; }
    }
}
