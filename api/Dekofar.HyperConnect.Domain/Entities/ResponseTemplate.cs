using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public class ResponseTemplate
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Body { get; set; } = default!;
        public bool IsGlobal { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Optional module scope such as "support", "returns", or "chat".
        /// </summary>
        public string? ModuleScope { get; set; }
    }
}
