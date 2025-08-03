using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public class UserUIPreference
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ModuleKey { get; set; } = default!;
        public string PreferenceJson { get; set; } = default!;
        public DateTime UpdatedAt { get; set; }
    }
}
