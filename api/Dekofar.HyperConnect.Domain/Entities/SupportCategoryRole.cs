using System;
using System.Text.Json.Serialization;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public class SupportCategoryRole
    {
        public Guid Id { get; set; }
        public Guid SupportCategoryId { get; set; }
        public string RoleName { get; set; } = default!;

        // Ignore back-reference to avoid recursive schemas in Swagger
        [JsonIgnore]
        public SupportCategory? Category { get; set; }
    }
}
