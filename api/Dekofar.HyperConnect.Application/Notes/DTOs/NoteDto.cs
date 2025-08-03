using System;

namespace Dekofar.HyperConnect.Application.Notes.DTOs
{
    public class NoteDto
    {
        public Guid Id { get; set; }
        public string TargetType { get; set; } = default!;
        public Guid TargetId { get; set; }
        public string Text { get; set; } = default!;
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
