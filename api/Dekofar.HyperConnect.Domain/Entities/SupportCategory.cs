using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Dekofar.HyperConnect.Domain.Entities
{
    public class SupportCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<SupportCategoryRole> Roles { get; set; } = new List<SupportCategoryRole>();

        // Prevent recursive schema generation with SupportTicket.Category
        [JsonIgnore]
        public ICollection<SupportTicket> Tickets { get; set; } = new List<SupportTicket>();
    }
}
