using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed
    }

    public class CalendarTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string AssignedUserId { get; set; } = string.Empty;
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
    }
}
