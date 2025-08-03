using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public class SupportTicketReply
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public SupportTicket Ticket { get; set; } = default!;
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = default!;
        public string Message { get; set; } = default!;
        public string? AttachmentUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
