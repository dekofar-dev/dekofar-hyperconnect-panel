using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public class PinCoverImage
    {
        public int Id { get; set; }
        public string Url { get; set; } = default!;
        public bool Active { get; set; }
        public TimeSpan? DisplayStartTime { get; set; }
        public TimeSpan? DisplayEndTime { get; set; }
    }
}
