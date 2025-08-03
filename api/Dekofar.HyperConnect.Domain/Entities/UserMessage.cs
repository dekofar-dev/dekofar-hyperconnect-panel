using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public class UserMessage
    {
        public Guid Id { get; set; }

        public Guid SenderId { get; set; }
        public ApplicationUser Sender { get; set; } = default!;

        public Guid ReceiverId { get; set; }
        public ApplicationUser Receiver { get; set; } = default!;

        public string? Text { get; set; }

        public string? FileUrl { get; set; }
        public string? FileType { get; set; }
        public long? FileSize { get; set; }

        public DateTime SentAt { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime? ReadAt { get; set; }
    }
}
