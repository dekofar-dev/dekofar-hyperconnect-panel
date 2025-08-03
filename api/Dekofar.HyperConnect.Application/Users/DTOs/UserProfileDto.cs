using System;

namespace Dekofar.HyperConnect.Application.Users.DTOs
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = default!;
        public string? AvatarUrl { get; set; }
        public string Email { get; set; } = default!;
        public int UnreadMessageCount { get; set; }
        public DateTime? LastMessageDate { get; set; }
        public DateTime? LastSupportActivityAt { get; set; }
        public bool IsOnline { get; set; }
    }
}
