using Dekofar.HyperConnect.Domain.Entities;
using System;

namespace Dekofar.HyperConnect.Application.SupportTickets.DTOs
{
    public class SupportTicketDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public Guid CreatedByUserId { get; set; }
        public Guid? AssignedUserId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? OrderId { get; set; }
        public SupportTicketStatus Status { get; set; }
        public SupportTicketPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public string? FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int UnreadReplyCount { get; set; }
    }
}
