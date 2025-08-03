using System;

namespace Dekofar.HyperConnect.Application.SupportTickets.DTOs
{
    public class SupportTicketReplyDto
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public Guid SenderId { get; set; }
        public Guid TicketOwnerId { get; set; }
        public string Message { get; set; } = default!;
        public string? AttachmentUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
