using System;

namespace Dekofar.HyperConnect.Application.Users.DTOs
{
    public class ProfileSummaryDto
    {
        public string FullName { get; set; } = default!;
        public string? AvatarUrl { get; set; }
        public DateTime JoinedAt { get; set; }

        public int TotalSupportTickets { get; set; }
        public int OpenSupportTickets { get; set; }
        public int ClosedSupportTickets { get; set; }
        public DateTime? LastSupportActivityAt { get; set; }

        public int TotalMessagesSent { get; set; }
        public int UnreadMessageCount { get; set; }
        public DateTime? LastMessageDate { get; set; }

        public decimal TotalSalesAmount { get; set; }
        public decimal TotalCommission { get; set; }

        public string[] Roles { get; set; } = Array.Empty<string>();
    }
}
